//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
//
// This code used to live at http://code.msdn.microsoft.com/LinqEntityDataReader. It was abandoned when the 
// Archive Gallery was retired and moved to 
// https://github.com/matthewschrager/Repository/blob/master/Repository.EntityFramework/EntityDataReader.cs 
// in 2013.
// Please visit Github for comments, issues and updates.
//
//Version 1.0.0.1 Added GetSchemaTable support for Loading DataTables from EntityDataReader
//Version 1.0.0.2 Added support for Entity Framework types, including Foreign Key columns
//Version 1.0.0.3 In DataReader.GetValue, now using dynamic methods for common scalar types instead of reflection with PropertyInfo.GetValue()
//Version 1.0.0.4 Added support for simple IEnumerable<T> where T is a scalar to support, eg, passing List<int> to a TVP
//Version 1.0.0.5 Simplified the Attribute code, added dynamic method support for all scalar types
//Version 1.0.0.6 Replaced the switch block for ValueAccessor with a Lambda Expression
//Version 1.0.0.6 Fixed a bug with nullable foreign keys on EF entities
//Version 1.0.0.6 Extensive refactoring, introduced EntityDataReaderOptions, changed constructors
//Version 1.0.0.6 Introduced option to flatten related entities.  Now you can have the EntityDataReader flatten an object graph.
//                This is especially useful for enabling you to project all the scalars of a related enty by just projecting the entity.
//                eg. a projection like new { salesOrder.ID, salesOrder.Customer, salesOrder.Product }                 
//                will create a DbDataReader with Id, Customer.*, Product.*
//Version 1.0.0.7 For anonymous types the order of columns is now controlled by the anonymous type's constructor arguments, for which 
//                Reflection tracks the ordinal position.  This ordering is applied to any type where the constructor args match the properties
//                1-1 on name and type.  Reflection over properties does not preserve the declaration order of the properties, and for Table Valued Parameters
//                SqlClient maps the DbDataReader columns to the TVP columns by ordinal position, not by name.  This relies on the behavior of 
//                the C# compiler that Anonymous types have a constructor with constructor arguments that match the object initializer on both name
//                and position.

using Autofac;
using DrakeQuest.DependencyInjection;
using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DrakeQuest.Data
{
	/// <summary>
	/// The EntityDataReader wraps a collection of CLR objects in a DbDataReader.  
	/// Only "scalar" properties are projected, with the exception that Entity Framework
	/// EntityObjects that have references to other EntityObjects will have key values for
	/// the related entity projected.
	/// 
	/// This is useful for doing high-speed data loads with SqlBulkCopy, and copying collections
	/// of entities to a DataTable for use with SQL Server Table-Valued parameters, or for interop
	/// with older ADO.NET applciations.
	/// 
	/// For explicit control over the fields projected by the DataReader, just wrap your collection
	/// of entities in a anonymous type projection before wrapping it in an EntityDataReader.
	/// 
	/// Instead of 
	/// IEnumerable<Order> orders;
	/// ...
	/// IDataReader dr = orders.AsDataReader();
	/// 
	/// do
	/// IEnumerable<Order> orders;
	/// ...
	/// var q = from o in orders
	///         select new 
	///         {
	///            ID=o.ID,
	///            ShipDate=o.ShipDate,
	///            ProductName=o.Product.Name,
	///            ...
	///         }
	/// IDataReader dr = q.AsDataReader();
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed partial class EntityDataReader<T> : DbDataReader, IDataReader
	{

		#region Fields

		readonly IEnumerator<T> enumerator;
		readonly EntityDataReaderOptions options;

		T current;
		bool closed = false;

		#endregion

		static List<Attribute> scalarAttributes;
		static List<Attribute> scalarAttributesPlusRelatedObjectScalarAttributes;
		static List<Attribute> scalarAttributesPlusRelatedObjectKeyAttributes;

		readonly List<Attribute> attributes;

		#region "Scalar Types"

		static bool IsScalarType(Type t)
		{
			return scalarTypes.Contains(t);
		}
		static readonly HashSet<Type> scalarTypes = LoadScalarTypes();
		static HashSet<Type> LoadScalarTypes()
		{
			HashSet<Type> set = new HashSet<Type>()
							  { 
                                //reference types
                                typeof(String),
								typeof(Byte[]),
                                //value types
                                typeof(Byte),
								typeof(Int16),
								typeof(Int32),
								typeof(Int64),
								typeof(Single),
								typeof(Double),
								typeof(Decimal),
								typeof(DateTime),
								typeof(Guid),
								typeof(Boolean),
								typeof(TimeSpan),
                                //nullable value types
                                typeof(Byte?),
								typeof(Int16?),
								typeof(Int32?),
								typeof(Int64?),
								typeof(Single?),
								typeof(Double?),
								typeof(Decimal?),
								typeof(DateTime?),
								typeof(Guid?),
								typeof(Boolean?),
								typeof(TimeSpan?)
							  };


			return set;

		}
		#endregion

		protected ILogger Logger { get; }

		#region Constructors

		public EntityDataReader(IEnumerable<T> col)
			: this(col, EntityDataReaderOptions.Default) { }

		public EntityDataReader(IEnumerable<T> col, EntityDataReaderOptions options)
		{
			Logger = Bootstrapper.GetInstance().Container.Resolve<ILogger<DbDataReader>>(new NamedParameter("loggerName", $"EntityDataReader<{typeof(T)}>"));
			this.enumerator = col.GetEnumerator();
			this.options = options;

			//done without a lock, so we risk running twice
			if (scalarAttributes == null)
			{
				scalarAttributes = DiscoverScalarAttributes(typeof(T));
			}
			if (options.FlattenRelatedObjects && scalarAttributesPlusRelatedObjectScalarAttributes == null)
			{
				var atts = DiscoverRelatedObjectScalarAttributes(typeof(T));
				scalarAttributesPlusRelatedObjectScalarAttributes = atts.Concat(scalarAttributes).ToList();
			}


			if (options.FlattenRelatedObjects)
			{
				attributes = scalarAttributesPlusRelatedObjectScalarAttributes;
			}
			else
			{
				attributes = scalarAttributes;
			}


		}


		static List<Attribute> DiscoverScalarAttributes(Type thisType)
		{

			//Not a collection of entities, just an IEnumerable<String> or other scalar type.
			//So add just a single Attribute that returns the object itself
			if (IsScalarType(thisType))
			{
				return new List<Attribute> { new Attribute("Value", "Value", thisType, t => t, false) };
			}


			//find all the scalar properties
			var allProperties = (from p in thisType.GetProperties()
								 where IsScalarType(p.PropertyType)
								 select p).ToList();

			//Look for a constructor with arguments that match the properties on name and type
			//(name modulo case, which varies between constructor args and properties in coding convention)
			//If such an "ordering constructor" exists, return the properties ordered by the corresponding
			//constructor args ordinal position.  
			//An important instance of an ordering constructor, is that C# anonymous types all have one.  So
			//this enables a simple convention to specify the order of columns projected by the EntityDataReader
			//by simply building the EntityDataReader from an anonymous type projection.
			//If such a constructor is found, replace allProperties with a collection of properties sorted by constructor order.
			foreach (var completeConstructor in from ci in thisType.GetConstructors()
												where ci.GetParameters().Count() == allProperties.Count()
												select ci)
			{
				var q = (from cp in completeConstructor.GetParameters()
						 join p in allProperties
						   on new { n = cp.Name.ToLower(), t = cp.ParameterType } equals new { n = p.Name.ToLower(), t = p.PropertyType }
						 select new { cp, p }).ToList();

				if (q.Count() == allProperties.Count()) //all constructor parameters matched by name and type to properties
				{
					//sort all properties by constructor ordinal position
					allProperties = (from o in q
									 orderby o.cp.Position
									 select o.p).ToList();
					break; //stop looking for an ordering constructor
				}


			}

			return allProperties.Select(p => new Attribute(p)).ToList();

		}
		static List<Attribute> DiscoverRelatedObjectScalarAttributes(Type thisType)
		{

			var atts = new List<Attribute>();

			//get the related objects which aren't scalars, not EntityReference objects and not collections
			var relatedObjectProperties =
							  (from p in thisType.GetProperties()
							   where !IsScalarType(p.PropertyType)
								  && !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType)

							   select p).ToList();

			foreach (var rop in relatedObjectProperties)
			{
				var type = rop.PropertyType;
				//get the scalar properties for the related type
				var scalars = type.GetProperties().Where(p => IsScalarType(p.PropertyType)).ToList();

				foreach (var sp in scalars)
				{
					string attName = rop.Name + "_" + sp.Name;
					//create a value accessor which takes an instance of T, and returns the related object scalar
					var valueAccessor = Attribute.MakeRelatedPropertyAccessor<T, object>(rop, sp);
					string name = attName;
					Attribute att = new Attribute(rop.Name, attName, sp.PropertyType, valueAccessor, true);
					atts.Add(att);
				}

			}
			return atts;

		}



		#endregion

		#region Utility Methods
		static Type nullable_T = typeof(System.Nullable<int>).GetGenericTypeDefinition();
		static bool IsNullable(Type t)
		{
			return (t.IsGenericType
				&& t.GetGenericTypeDefinition() == nullable_T);
		}
		static Type StripNullableType(Type t)
		{
			return t.GetGenericArguments()[0];
		}
		#endregion

		#region GetSchemaTable


		const string shemaTableSchema = @"<?xml version=""1.0"" standalone=""yes""?>
<xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
  <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""SchemaTable"" msdata:Locale="""">
    <xs:complexType>
      <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
        <xs:element name=""SchemaTable"" msdata:Locale="""" msdata:MinimumCapacity=""1"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""ColumnName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""ColumnOrdinal"" msdata:ReadOnly=""true"" type=""xs:int"" default=""0"" minOccurs=""0"" />
              <xs:element name=""ColumnSize"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
              <xs:element name=""NumericPrecision"" msdata:ReadOnly=""true"" type=""xs:short"" minOccurs=""0"" />
              <xs:element name=""NumericScale"" msdata:ReadOnly=""true"" type=""xs:short"" minOccurs=""0"" />
              <xs:element name=""IsUnique"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsKey"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""BaseServerName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""BaseCatalogName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""BaseColumnName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""BaseSchemaName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""BaseTableName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""DataType"" msdata:DataType=""System.Type, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""AllowDBNull"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""ProviderType"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
              <xs:element name=""IsAliased"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsExpression"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsIdentity"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsAutoIncrement"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsRowVersion"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsHidden"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""IsLong"" msdata:ReadOnly=""true"" type=""xs:boolean"" default=""false"" minOccurs=""0"" />
              <xs:element name=""IsReadOnly"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
              <xs:element name=""ProviderSpecificDataType"" msdata:DataType=""System.Type, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""DataTypeName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""XmlSchemaCollectionDatabase"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""XmlSchemaCollectionOwningSchema"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""XmlSchemaCollectionName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""UdtAssemblyQualifiedName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
              <xs:element name=""NonVersionedProviderType"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:choice>
    </xs:complexType>
  </xs:element>
</xs:schema>";
		public override DataTable GetSchemaTable()
		{
			DataSet s = new DataSet();
			s.Locale = System.Globalization.CultureInfo.CurrentCulture;
			s.ReadXmlSchema(new System.IO.StringReader(shemaTableSchema));
			DataTable t = s.Tables[0];
			for (int i = 0; i < this.FieldCount; i++)
			{
				DataRow row = t.NewRow();
				row["ColumnName"] = this.GetName(i);
				row["ColumnOrdinal"] = i;

				Type type  = attributes[i].Type; ;
				if (IsNullable(type))
				{
					row["AllowDBNull"] = true;
					//type = type.GetGenericArguments()[0];
				}
				row["DataType"] = type;
				row["DataTypeName"] = this.GetDataTypeName(i);
				
				row["ColumnSize"] = -1;
				t.Rows.Add(row);
			}
			return t;

		}
		#endregion

		#region IDataReader Members

		public override void Close()
		{
			closed = true;
		}

		public override int Depth
		{
			get { return 1; }
		}


		public override bool IsClosed
		{
			get { return closed; }
		}

		public override bool NextResult()
		{
			return false;
		}

		int entitiesRead = 0;
		public override bool Read()
		{
			bool rv = enumerator.MoveNext();
			if (rv)
			{
				current = enumerator.Current;
				entitiesRead += 1;
			}
			return rv;
		}

		public override int RecordsAffected
		{
			get { return -1; }
		}

		#endregion

		#region IDisposable Members

		protected override void Dispose(bool disposing)
		{
			Close();
			base.Dispose(disposing);
		}

		#endregion

		#region IDataRecord Members

		public override int FieldCount
		{
			get
			{
				return attributes.Count;
			}
		}

		TField GetValue<TField>(int i)
		{
			Logger.Log(string.Format("{0}.GetValue<{2}>({1})", current, i,typeof(TField) ));
			TField val = (TField)attributes[i].GetValue(current);
			return val;
		}
		public override bool GetBoolean(int i)
		{
			return GetValue<bool>(i);
		}

		public override byte GetByte(int i)
		{
			return GetValue<byte>(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{

			var buf = GetValue<byte[]>(i);
			int bytes = Math.Min(length, buf.Length - (int)fieldOffset);
			Buffer.BlockCopy(buf, (int)fieldOffset, buffer, bufferoffset, bytes);
			return bytes;

		}

		public override char GetChar(int i)
		{
			return GetValue<char>(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			//throw new NotImplementedException();
			string s = GetValue<string>(i);
			int chars = Math.Min(length, s.Length - (int)fieldoffset);
			s.CopyTo((int)fieldoffset, buffer, bufferoffset, chars);

			return chars;
		}

		//public override DbDataReader GetData(int i)
		//{
		//  throw new NotImplementedException();
		//}

		public override string GetDataTypeName(int i)
		{
			return GetFieldType(i).Name;
		}

		public override DateTime GetDateTime(int i)
		{
			return GetValue<DateTime>(i);
		}

		public override decimal GetDecimal(int i)
		{
			return GetValue<decimal>(i);
		}

		public override double GetDouble(int i)
		{
			return GetValue<double>(i);
		}

		public override Type GetFieldType(int i)
		{
			Type t = attributes[i].Type;
			if (IsNullable(t))
			{
				return StripNullableType(t);
			}
			return attributes[i].Type;
		}

		public override float GetFloat(int i)
		{
			return GetValue<float>(i);
		}

		public override Guid GetGuid(int i)
		{
			return GetValue<Guid>(i);
		}

		public override short GetInt16(int i)
		{
			return GetValue<short>(i);
		}

		public override int GetInt32(int i)
		{
			return GetValue<int>(i);
		}

		public override long GetInt64(int i)
		{
			return GetValue<long>(i);
		}

		public override string GetName(int i)
		{
			Attribute a = attributes[i];
			if (a.IsRelatedAttribute && options.PrefixRelatedObjectColumns)
			{
				return a.FullName;
			}
			return a.Name;
		}

		public override int GetOrdinal(string name)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				var a = attributes[i];

				if (!a.IsRelatedAttribute && a.Name == name)
				{
					return i;
				}

				if (options.PrefixRelatedObjectColumns && a.IsRelatedAttribute && a.FullName == name)
				{
					return i;
				}

				if (!options.PrefixRelatedObjectColumns && a.IsRelatedAttribute && a.Name == name)
				{
					return i;
				}


			}
			return -1;
		}

		public override string GetString(int i)
		{
			return GetValue<string>(i);
		}



		public override int GetValues(object[] values)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				values[i] = GetValue(i);
			}
			return attributes.Count;
		}

		public override object GetValue(int i)
		{
			object o = GetValue<object>(i);
			if (!options.ExposeNullableTypes && o == null)
			{
				return DBNull.Value;
			}
			return o;
		}

		public override bool IsDBNull(int i)
		{
			object o = GetValue<object>(i);
			return (o == null);
		}

		public override object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}

		public override object this[int i]
		{
			get { return GetValue(i); }
		}

		#endregion

		#region DbDataReader Members



		public override System.Collections.IEnumerator GetEnumerator()
		{
			return this.enumerator;
		}

		public override bool HasRows
		{
			get { throw new NotSupportedException(); }
		}
		#endregion

	}
}