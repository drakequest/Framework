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
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

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
		private class Attribute
		{
			//PropertyInfo propertyInfo;
			public readonly Type Type;
			public readonly string FullName;
			public readonly string Name;
			public readonly bool IsRelatedAttribute;

			readonly Func<T, object> ValueAccessor;

			/// <summary>
			/// Uses Lamda expressions to create a Func<T,object> that invokes the given property getter.
			/// The property value will be extracted and cast to type TProperty
			/// </summary>
			/// <typeparam name="TObject">The type of the object declaring the property.</typeparam>
			/// <typeparam name="TProperty">The type to cast the property value to</typeparam>
			/// <param name="pi">PropertyInfo pointing to the property to wrap</param>
			/// <returns></returns>
			public static Func<TObject, TProperty> MakePropertyAccessor<TObject, TProperty>(PropertyInfo pi)
			{
				ParameterExpression objParam = Expression.Parameter(typeof(TObject), "obj");
				MemberExpression typedAccessor = Expression.PropertyOrField(objParam, pi.Name);
				UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(object));
				LambdaExpression lambdaExpr = Expression.Lambda<Func<TObject, TProperty>>(castToObject, objParam);

				return (Func<TObject, TProperty>)lambdaExpr.Compile();
			}


			public static Func<TObject, TProperty> MakeRelatedPropertyAccessor<TObject, TProperty>(PropertyInfo pi, PropertyInfo pi2)
			{

				Func<TObject, object> getRelatedObject;
				{
					// expression like:
					//    return (object)t.SomeProp;
					ParameterExpression typedParam = Expression.Parameter(typeof(T), "t");
					MemberExpression typedAccessor = Expression.PropertyOrField(typedParam, pi.Name);
					UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(object));
					LambdaExpression lambdaExpr = Expression.Lambda<Func<TObject, object>>(castToObject, typedParam);
					getRelatedObject = (Func<TObject, object>)lambdaExpr.Compile();
				}


				Func<object, TProperty> getRelatedObjectProperty;
				{

					// expression like:
					//    return (object)((PropType)o).RelatedProperty;
					ParameterExpression objParam = Expression.Parameter(typeof(object), "o");
					UnaryExpression typedParam = Expression.Convert(objParam, pi.PropertyType);
					MemberExpression typedAccessor = Expression.PropertyOrField(typedParam, pi2.Name);
					UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(TProperty));
					LambdaExpression lambdaExpr = Expression.Lambda<Func<object, TProperty>>(castToObject, objParam);
					getRelatedObjectProperty = (Func<object, TProperty>)lambdaExpr.Compile();
				}

				Func<TObject, TProperty> f = (TObject t) =>
				{
					object o = getRelatedObject(t);
					if (o == null) return default(TProperty);
					return getRelatedObjectProperty(o);
				};

				return f;
			}

			public Attribute(PropertyInfo pi)
			{
				this.FullName = pi.DeclaringType.Name + "_" + pi.Name;
				this.Name = pi.Name;
				Type = pi.PropertyType;
				IsRelatedAttribute = false;

				ValueAccessor = MakePropertyAccessor<T, object>(pi);
			}

			public Attribute(string fullName, string name, Type type, Func<T, object> getValue, bool isRelatedAttribute)
			{
				this.FullName = fullName;
				this.Name = name;
				this.Type = type;
				this.ValueAccessor = getValue;
				this.IsRelatedAttribute = isRelatedAttribute;
			}

			public object GetValue(T target)
			{
				return ValueAccessor(target);
			}
		}
	}
}