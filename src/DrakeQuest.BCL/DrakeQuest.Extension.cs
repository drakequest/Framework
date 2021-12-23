using DrakeQuest.Data.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DrakeQuest
{
	public static class TypeExtensions
	{
		static Type nullable_T = typeof(System.Nullable<int>).GetGenericTypeDefinition();

		public static Type GetTypeExt(this Type type)
		{
			if (type == null)
				return null;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return  type.GetGenericArguments()[0];
			}
			return type;
		}

		#region Utility Methods

		public static bool IsNullable(this Type t)
		{
			return (t.IsGenericType
				&& t.GetGenericTypeDefinition() == nullable_T) || t == typeof(string);
		}
		public static Type StripNullableType(this Type t)
		{
			return t.GetGenericArguments()[0];
		}
		#endregion

		/// <summary>
		/// Get all the properties to map to the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static  IEnumerable<IMapperPropertyInfo> GetPropertyMetaInfos(this Type type, Func<PropertyInfo, IMapperPropertyInfo> mapperPropertyInfoFactory,  bool onlyWithMeta = false )
        {

			if (type == null)
				return new List<IMapperPropertyInfo>();

			var propertyInfos =
				type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Select(prop => mapperPropertyInfoFactory(prop));

			if (onlyWithMeta)
			{
				var metas = propertyInfos
							.Where(att => att.MetaInformation != null).ToList();
				return metas;

			}

			if (propertyInfos.Any(prop => prop.MetaInformation != null))
			{
				return propertyInfos.Where(prop => prop.MetaInformation != null)
					.OrderBy(col => col.Order)
					.ThenBy(col => col.Name)
					.ToList();
			}

			return propertyInfos
				.OrderBy( meta=> meta.Name)
				.ToList();
		}
	}
}
