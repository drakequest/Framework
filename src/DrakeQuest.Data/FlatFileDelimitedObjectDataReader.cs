using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using DrakeQuest.Data.Mapper;

namespace DrakeQuest.Data
{
	public class FlatFileObjectDataReader<T> : ObjectDataReader
	{
		#region fields

		new public const string LoggerName = "[FlatFileObjectDataAdapter]";

		#endregion

		#region properties

		protected object Current { get; set; }

		#endregion

		#region Constructor
		public FlatFileObjectDataReader(ILogger<ObjectDataReader> logger, Func<PropertyInfo, IMapperPropertyInfo> mapperFactory, IEnumerable collection)
			: base(logger, mapperFactory, collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "The collection can't be null");

			Enumerator = collection.GetEnumerator();
			if (Enumerator.MoveNext())
			{
				RowType = Enumerator.Current.GetType();
				Initialize(RowType);
			}
			Enumerator = collection.GetEnumerator();
		}

		#endregion

		#region Methods

		protected void Initialize(Type rowType)
		{
			var props = GetPropertyInfos(rowType)
			.Where(p=> p.PropertyInfo.PropertyType.GetTypeExt().IsValueType || p.PropertyInfo.PropertyType.GetTypeExt() == typeof(string))
			.Where(p=> !p.PropertyInfo.GetCustomAttributes().Any() ||
				 !p.PropertyInfo.GetCustomAttributes().Any(at=> at.GetType().Name.ToLower().Contains("ignore")))
				.ToList();
			_FieldCount = props.Count;

			for (int i = FieldCount - 1; i > -1; i--)
			{
				var prop = props[i];
				AddFields(prop.Order ?? i, prop.Name, prop.PropertyInfo);
			}

		}

		protected virtual void AddFields(int i, string name, PropertyInfo propertyInfo)
		{
			ColumnNames.Add(name, propertyInfo);
			ColumnIndex.Add(i, propertyInfo);
			IndexToName.Add(i, name);
			NameToIndex.Add(name, i);
		}

		public override void Close()
		{
		}

		protected override void Dispose(bool disposing)
		{
			ColumnNames.Clear();
			ColumnIndex.Clear();
			IndexToName.Clear();
			NameToIndex.Clear();
		}


		public override string GetDataTypeName(int i)
		{
			return ColumnIndex[i].PropertyType.GetTypeExt().Name;
		}

		public override Type GetFieldType(int i)
		{
			return  ColumnIndex[i].PropertyType.GetTypeExt() ;
		}

		public override string GetName(int i)
		{
			try
			{
				return IndexToName[i];
			}
			catch (Exception ex)
			{
				Logger.LogFormat(LevelEnum.Fatal, "key not found {0}", i);
				throw;
			}
		}

		public override int GetOrdinal(string name)
		{
			try
			{
				return NameToIndex[name];
			}
			catch (Exception ex)
			{
				Logger.LogFormat(LevelEnum.Fatal,"key not found {0}", name);
				throw;
			}
		}

		public override bool NextResult()
		{
			return false;
		}

		public override bool Read()
		{
			var result = Enumerator.MoveNext();
			if (result)
				Current = Enumerator.Current;
			return result;
		}

		public override object GetValue(int ordinal)
		{
			var result = ColumnIndex[ordinal].GetValue(Current) ?? DBNull.Value;
			return result;
		}

		private object GetValue(string name)
		{
			var result = ColumnNames[name].GetValue(Current) ?? DBNull.Value;
			return result;
		}

		public static object GetDefault(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		#endregion
	}
}