using DrakeQuest.Log;
using DrakeQuest.Log.Generic;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using DrakeQuest.Data.Mapper;

namespace DrakeQuest.Data
{
	public class ObjectDataReader : DbDataReader, IObjectDataReader
	{
		#region fields

		protected int _FieldCount;

		public const string LoggerName = "[ObjectDataAdapter]";

		#endregion

		#region properties

		protected Dictionary<string, PropertyInfo> ColumnNames { get; }

		protected Dictionary<int, PropertyInfo> ColumnIndex { get; }

		protected Dictionary<int, string> IndexToName { get; }

		protected Dictionary<string, int> NameToIndex { get; }

		protected Type RowType { get; set; }

		protected ILogger Logger { get; }

        protected Func<PropertyInfo, IMapperPropertyInfo> MapperPropertyInfoFactory { get; }

        public override object this[string name]
		{
			get { return GetValue(name); }
		}

		public override object this[int index]
		{
			get { return GetValue(index); }
		}

		public override int FieldCount
		{
			get { return _FieldCount;  }
		}

		protected IEnumerator Enumerator { get; set; }

		protected object Current { get; set; }

		#endregion

		#region Constructor

		private ObjectDataReader(ILogger<ObjectDataReader> logger, Func<PropertyInfo, IMapperPropertyInfo> mapperFactory)
		{
			ColumnNames = new Dictionary<string, PropertyInfo>();
			ColumnIndex = new Dictionary<int, PropertyInfo>();
			IndexToName = new Dictionary<int, string>();
			NameToIndex = new Dictionary<string, int>();
			Logger = logger;
		}

		public ObjectDataReader(ILogger<ObjectDataReader> logger, Func<PropertyInfo, IMapperPropertyInfo> mapperFactory, IEnumerable collection)
			: this(logger, mapperFactory)
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


		/// <summary>
		/// Get all the properties to map to the reader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected virtual IEnumerable<IMapperPropertyInfo> GetPropertyInfos(Type type)
		{
            return null; // type.GetPropertyMetaInfos(MapperPropertyInfoFactory);
		}


		public override void Close() { }

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

		public override DataTable GetSchemaTable()
		{

			var tempSchemaTable = GetBaseSchemaTableFromDataTable();
			if (RowType == null)
				return tempSchemaTable;

			Type rowType = RowType;
			var properties = GetPropertyInfos(rowType);

			tempSchemaTable.Columns[SchemaTableColumn.BaseTableName].DefaultValue = rowType.Name;
			//BaseTableNamespace.DefaultValue = table.Namespace;

			foreach (var dc in properties.Select(o => o.PropertyInfo))
			{
				DataRow dr = tempSchemaTable.NewRow();

				dr[SchemaTableColumn.ColumnName] = dc.Name;
				dr[SchemaTableColumn.ColumnOrdinal] = GetOrdinal(dc.Name);
				dr[SchemaTableColumn.DataType] = dc.PropertyType.GetTypeExt();

				if (dc.PropertyType == typeof(string))
					dr[SchemaTableColumn.ColumnSize] = int.MaxValue;

				dr[SchemaTableColumn.AllowDBNull] = dc.PropertyType.IsNullable();
				dr[SchemaTableOptionalColumn.IsReadOnly] = true;

				dr[SchemaTableOptionalColumn.DefaultValue] = GetDefault(dc.PropertyType) ?? DBNull.Value;

				//dr[SchemaTableOptionalColumn.ColumnMapping] = dc.ColumnMapping;
				//dr[SchemaTableColumn.BaseColumnName] = dc.ColumnName;

				tempSchemaTable.Rows.Add(dr);
			}

			tempSchemaTable.AcceptChanges();

			return tempSchemaTable;
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