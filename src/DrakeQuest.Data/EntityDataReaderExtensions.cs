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


using System;
using System.Collections.Generic;
using System.Data;
using Autofac;
using DrakeQuest.DependencyInjection;

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
	public static class EntityDataReaderExtensions
	{
		private static Dictionary<Type, DbType> TypeMap;


		static EntityDataReaderExtensions()
		{
			TypeMap = GetTypeMap();
		}

		public static DbType AsDbType(this Type type)
		{
			return TypeMap[type];
		}

		public static Dictionary<Type, DbType> GetTypeMap()

		
		{
			var typeMap = new Dictionary<Type, DbType>();
			typeMap[typeof(byte)] = DbType.Byte;
			typeMap[typeof(sbyte)] = DbType.SByte;
			typeMap[typeof(short)] = DbType.Int16;
			typeMap[typeof(ushort)] = DbType.UInt16;
			typeMap[typeof(int)] = DbType.Int32;
			typeMap[typeof(uint)] = DbType.UInt32;
			typeMap[typeof(long)] = DbType.Int64;
			typeMap[typeof(ulong)] = DbType.UInt64;
			typeMap[typeof(float)] = DbType.Single;
			typeMap[typeof(double)] = DbType.Double;
			typeMap[typeof(decimal)] = DbType.Decimal;
			typeMap[typeof(bool)] = DbType.Boolean;
			typeMap[typeof(string)] = DbType.String;
			typeMap[typeof(char)] = DbType.StringFixedLength;
			typeMap[typeof(Guid)] = DbType.Guid;
			typeMap[typeof(DateTime)] = DbType.DateTime;
			typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
			typeMap[typeof(byte[])] = DbType.Binary;
			typeMap[typeof(byte?)] = DbType.Byte;
			typeMap[typeof(sbyte?)] = DbType.SByte;
			typeMap[typeof(short?)] = DbType.Int16;
			typeMap[typeof(ushort?)] = DbType.UInt16;
			typeMap[typeof(int?)] = DbType.Int32;
			typeMap[typeof(uint?)] = DbType.UInt32;
			typeMap[typeof(long?)] = DbType.Int64;
			typeMap[typeof(ulong?)] = DbType.UInt64;
			typeMap[typeof(float?)] = DbType.Single;
			typeMap[typeof(double?)] = DbType.Double;
			typeMap[typeof(decimal?)] = DbType.Decimal;
			typeMap[typeof(bool?)] = DbType.Boolean;
			typeMap[typeof(char?)] = DbType.StringFixedLength;
			typeMap[typeof(Guid?)] = DbType.Guid;
			typeMap[typeof(DateTime?)] = DbType.DateTime;
			typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;

#if !NETCOREAPP
			typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
#endif
			return typeMap;
		}

		public static IDataReader AsDataReader<T>(this IEnumerable<T> collection)
		{
			return Bootstrapper.GetContainer().Resolve<IObjectDataReader>(new NamedParameter("collection", collection));
		}

		/// <summary>
		/// Wraps the IEnumerable in a DbDataReader, having one column for each "scalar" property of the type T.  
		/// The collection will be enumerated as the client calls IDataReader.Read().
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static IDataReader AsEntityDataReader<T>(this IEnumerable<T> collection, bool exposeNullableColumns, bool flattenRelatedObjects)
		{
			EntityDataReaderOptions options = new EntityDataReaderOptions(exposeNullableColumns, flattenRelatedObjects, true);
			return new EntityDataReader<T>(collection, options);
		}


		/// <summary>
		/// Enumerates the collection and copies the data into a DataTable.
		/// </summary>
		/// <typeparam name="T">The element type of the collection.</typeparam>
		/// <param name="collection">The collection to copy to a DataTable</param>
		/// <returns>A DataTable containing the scalar projection of the collection.</returns>
		public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
		{
			DataTable t = new DataTable();
			t.Locale = System.Globalization.CultureInfo.CurrentCulture;
			t.TableName = typeof(T).Name;
			EntityDataReaderOptions options = EntityDataReaderOptions.Default;
			options.ExposeNullableTypes = false;
			EntityDataReader<T> dr = new EntityDataReader<T>(collection, options);
			t.Load(dr);
			return t;
		}

	}
}