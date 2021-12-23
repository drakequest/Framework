using System;
using System.Collections.Generic;
using System.Data;
using DrakeQuest.Data.Properties;
using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using System.Reflection;

namespace DrakeQuest.Data.Mapper
{
	internal class DataReaderObjectMapper : IDataReaderObjectMapper
	{
		#region Fields

		public const string LoggerName = "[Data.ObjectMapper]";

		#endregion

		#region Properties

		public Dictionary<int, IMapperInfo> Properties { get; set; }

		public IMapperInfo this[int i]
		{
			get
			{
				return Properties[i];
			}
		}

		public int FieldCount { get; set; }

		public ILogger Logger { get; private set; }

        protected Func<PropertyInfo, IMapperPropertyInfo> MapperPropertyInfoFactory { get; }

        #endregion

        #region Constructor

        public DataReaderObjectMapper(ILogger<DataReaderObjectMapper> logger)
		{
			Logger = logger;
		}

		#endregion

		#region Methods

		public void Initialize<T>(IDataReader reader)
		{
			var columns = new Dictionary<string, int>(reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
			for (int i = 0; i < reader.FieldCount; i++)
			{
				columns.Add(reader.GetName(i), i);
			}

			Properties = new Dictionary<int, IMapperInfo>(columns.Count);
			var properties = GetPropertyInfos<T>();

			foreach (var prop in properties)
			{
				string columnName = prop.Name;
				if (!columns.ContainsKey(columnName))
					continue;

				int i = reader.GetOrdinal(columnName);

				Type propType = prop.PropertyInfo.PropertyType;
				Type fieldType = reader.GetFieldType(i);
				if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					propType = propType.GetGenericArguments()[0];
				}

				Properties.Add(i, GetMapperInfo(prop, fieldType, propType));
			}
			FieldCount = reader.FieldCount;
		}

		public bool ContainsKey(int i)
		{
			return Properties.ContainsKey(i);
		}

		public virtual IMapperInfo GetMapperInfo(IMapperPropertyInfo propertyInfo, Type fieldType, Type propType)
		{
			return  new MapperInfo(propertyInfo.PropertyInfo);
		}

		/// <summary>
		/// Get all the properties to map to the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual IEnumerable<IMapperPropertyInfo> GetPropertyInfos<T>()
		{
            IEnumerable<IMapperPropertyInfo> results = typeof (T).GetPropertyMetaInfos(mapperPropertyInfoFactory:MapperPropertyInfoFactory,onlyWithMeta: true);
			return results;
		}

		public virtual void Populate<T>(ref T data, IDataReader reader)
		{
			for (int i = 0; i < FieldCount; i++)
			{
				if (reader.IsDBNull(i))
					continue;

				if (!ContainsKey(i))
					continue;

				var propMapper = this[i];
				var prop = propMapper.PropertyInfo;
				try
				{
					prop.SetValue(data, propMapper.GetValue(reader, i), null);
				}
				catch (Exception ex)
				{
					Logger.LogFormat(LevelEnum.Warn, ex, Resources.ERR_MAPPING_PROP, prop.Name, reader.GetValue(i), ex.Message);
				}
			}
		}

		#endregion
	}
}
