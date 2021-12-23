using System;
using System.Collections.Generic;
using System.Data;

namespace DrakeQuest.Data.Mapper
{
	public interface IDataReaderObjectMapper
	{
		Dictionary<int, IMapperInfo> Properties { get; set; }

		int FieldCount { get; set; }

		IMapperInfo this[int i] { get; }
		
		void Initialize<T>(IDataReader reader);
		
		bool ContainsKey(int i);

		IMapperInfo GetMapperInfo(IMapperPropertyInfo propertyInfo, Type fieldType, Type propType);

		/// <summary>
		/// Get all the properties to map to the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<IMapperPropertyInfo> GetPropertyInfos<T>();

		void Populate<T>(ref T data, IDataReader reader);
	}
}
