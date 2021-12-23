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
	public class EntityDataReaderOptions
	{
		public static EntityDataReaderOptions Default
		{
			get { return new EntityDataReaderOptions(true, false, false); }
		}

		public EntityDataReaderOptions(
		  bool exposeNullableTypes,
		  bool flattenRelatedObjects,
		  bool prefixRelatedObjectColumns)
		{
			this.ExposeNullableTypes = exposeNullableTypes;
			this.FlattenRelatedObjects = flattenRelatedObjects;
			this.PrefixRelatedObjectColumns = prefixRelatedObjectColumns;
		}

		/// <summary>
		/// If true nullable value types are returned directly by the DataReader.
		/// If false, the DataReader will expose non-nullable value types and return DbNull.Value
		/// for null values.  
		/// When loading a DataTable this option must be set to True, since DataTable does not support
		/// nullable types.
		/// </summary>
		public bool ExposeNullableTypes { get; set; }

		/// <summary>
		/// If True then the DataReader will project scalar properties from related objects in addition
		/// to scalar properties from the main object.  This is especially useful for custom projecttions like
		///         var q = from od in db.SalesOrderDetail
		///         select new
		///         {
		///           od,
		///           ProductID=od.Product.ProductID,
		///           ProductName=od.Product.Name
		///         };
		/// Related objects assignable to EntityKey, EntityRelation, and IEnumerable are excluded.
		/// 
		/// If False, then only scalar properties from teh main object will be projected.         
		/// </summary>
		public bool FlattenRelatedObjects { get; set; }

		/// <summary>
		/// If True columns projected from related objects will have column names prefixed by the
		/// name of the relating property.  This appies to either from setting FlattenRelatedObjects to True,
		/// or RecreateForeignKeysForEntityFrameworkEntities to True.
		/// 
		/// If False columns will be created for related properties that are not prefixed.  This can lead
		/// to column name collision.
		/// </summary>
		public bool PrefixRelatedObjectColumns { get; set; }

		/// <summary>
		/// If True the DataReader will create columns for the key properties of related Entities.
		/// You must pass an ObjectContext and have retrieved the entity with change tracking for this to work.
		/// </summary>
		//public bool RecreateForeignKeysForEntityFrameworkEntities { get; set; }

	}
}