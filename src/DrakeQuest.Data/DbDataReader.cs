using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace DrakeQuest.Data
{
	public abstract class DbDataReader : System.Data.Common.DbDataReader
	{


        #region properties

        public override int FieldCount { get; }

		public override bool HasRows { get; }

		public override bool IsClosed { get; }


		#region IDataReader

		public override int RecordsAffected { get { return -1; } }

		public override int Depth { get { return -1; } }

		#endregion

		#region IDataRecord

		/// <summary>
		/// Gets the column with the specified name.
		/// </summary>
		/// 
		/// <returns>
		/// The column with the specified name as an <see cref="T:System.Object"/>.
		/// </returns>
		/// <param name="name">The name of the column to find. </param><exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception><filterpriority>2</filterpriority>
		public override object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}

		/// <summary>
		/// Gets the column located at the specified index.
		/// </summary>
		/// 
		/// <returns>
		/// The column located at the specified index as an <see cref="T:System.Object"/>.
		/// </returns>
		/// <param name="i">The zero-based index of the column to get. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
		public override object this[int i]
		{
			get { return GetValue(i); }
		}

		#endregion

		#endregion

		#region Methods

		#region data getter

		public override bool GetBoolean(int i)
		{
			return GetFieldValue<bool>(i);
		}

		public override byte GetByte(int i)
		{
			return GetFieldValue<byte>(i);
		}

		public override char GetChar(int i)
		{
			return GetFieldValue<char>(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return GetFieldValue<DateTime>(i);
		}

		public override decimal GetDecimal(int i)
		{
			return GetFieldValue<decimal>(i);
		}

		public override double GetDouble(int i)
		{
			return GetFieldValue<double>(i);
		}

		public override float GetFloat(int i)
		{
			return GetFieldValue<float>(i);
		}

		public override Guid GetGuid(int i)
		{
			return GetFieldValue<Guid>(i);
		}

		public override short GetInt16(int i)
		{
			return GetFieldValue<short>(i);
		}

		public override int GetInt32(int i)
		{
			return GetFieldValue<int>(i);
		}

		public override long GetInt64(int i)
		{
			return GetFieldValue<long>(i);
		}

		public override string GetString(int i)
		{
			return GetFieldValue<string>(i);
		}

		#endregion

		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public override int GetValues(object[] values)
		{
			if (values == null || values.Length == 0)
				return 0;
			int length = Math.Min(values.Length, FieldCount);

			for (int i = length -1; i >  -1; i--)
				values[i] = GetValue(i);
			return length;
		}

		public override bool IsDBNull(int i)
		{
			return GetValue(i) == null;
		}

		public override bool NextResult()
		{
			return false;
		}

		#endregion

		public override DataTable GetSchemaTable()
		{
			return GetBaseSchemaTableFromDataTable();
		}

		protected static DataTable GetBaseSchemaTableFromDataTable()
		{
			DataTable tempSchemaTable = new DataTable("SchemaTable");
			tempSchemaTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

			DataColumn ColumnName = new DataColumn(SchemaTableColumn.ColumnName, typeof(System.String));
			DataColumn ColumnOrdinal = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(System.Int32));
			DataColumn ColumnSize = new DataColumn(SchemaTableColumn.ColumnSize, typeof(System.Int32));
			DataColumn NumericPrecision = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(System.Int16));
			DataColumn NumericScale = new DataColumn(SchemaTableColumn.NumericScale, typeof(System.Int16));
			DataColumn DataType = new DataColumn(SchemaTableColumn.DataType, typeof(System.Type));
			DataColumn ProviderType = new DataColumn(SchemaTableColumn.ProviderType, typeof(System.Int32));
			DataColumn IsLong = new DataColumn(SchemaTableColumn.IsLong, typeof(System.Boolean));
			DataColumn AllowDBNull = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(System.Boolean));
			DataColumn IsReadOnly = new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(System.Boolean));
			DataColumn IsRowVersion = new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(System.Boolean));
			DataColumn IsUnique = new DataColumn(SchemaTableColumn.IsUnique, typeof(System.Boolean));
			DataColumn IsKeyColumn = new DataColumn(SchemaTableColumn.IsKey, typeof(System.Boolean));
			DataColumn IsAutoIncrement = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(System.Boolean));
			DataColumn BaseSchemaName = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(System.String));
			DataColumn BaseCatalogName = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(System.String));
			DataColumn BaseTableName = new DataColumn(SchemaTableColumn.BaseTableName, typeof(System.String));
			DataColumn BaseColumnName = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(System.String));
			DataColumn AutoIncrementSeed = new DataColumn(SchemaTableOptionalColumn.AutoIncrementSeed, typeof(System.Int64));
			DataColumn AutoIncrementStep = new DataColumn(SchemaTableOptionalColumn.AutoIncrementStep, typeof(System.Int64));
			DataColumn DefaultValue = new DataColumn(SchemaTableOptionalColumn.DefaultValue, typeof(System.Object));
			DataColumn Expression = new DataColumn(SchemaTableOptionalColumn.Expression, typeof(System.String));
			DataColumn ColumnMapping = new DataColumn(SchemaTableOptionalColumn.ColumnMapping, typeof(System.Data.MappingType));
			DataColumn BaseTableNamespace = new DataColumn(SchemaTableOptionalColumn.BaseTableNamespace, typeof(System.String));
			DataColumn BaseColumnNamespace = new DataColumn(SchemaTableOptionalColumn.BaseColumnNamespace, typeof(System.String));

			ColumnSize.DefaultValue = -1;


			IsRowVersion.DefaultValue = false;
			IsLong.DefaultValue = false;
			IsReadOnly.DefaultValue = false;
			IsKeyColumn.DefaultValue = false;
			IsAutoIncrement.DefaultValue = false;
			AutoIncrementSeed.DefaultValue = 0;
			AutoIncrementStep.DefaultValue = 1;


			tempSchemaTable.Columns.Add(ColumnName);
			tempSchemaTable.Columns.Add(ColumnOrdinal);
			tempSchemaTable.Columns.Add(ColumnSize);
			tempSchemaTable.Columns.Add(NumericPrecision);
			tempSchemaTable.Columns.Add(NumericScale);
			tempSchemaTable.Columns.Add(DataType);
			tempSchemaTable.Columns.Add(ProviderType);
			tempSchemaTable.Columns.Add(IsLong);
			tempSchemaTable.Columns.Add(AllowDBNull);
			tempSchemaTable.Columns.Add(IsReadOnly);
			tempSchemaTable.Columns.Add(IsRowVersion);
			tempSchemaTable.Columns.Add(IsUnique);
			tempSchemaTable.Columns.Add(IsKeyColumn);
			tempSchemaTable.Columns.Add(IsAutoIncrement);
			tempSchemaTable.Columns.Add(BaseCatalogName);
			tempSchemaTable.Columns.Add(BaseSchemaName);
			// specific to datatablereader
			tempSchemaTable.Columns.Add(BaseTableName);
			tempSchemaTable.Columns.Add(BaseColumnName);
			tempSchemaTable.Columns.Add(AutoIncrementSeed);
			tempSchemaTable.Columns.Add(AutoIncrementStep);
			tempSchemaTable.Columns.Add(DefaultValue);
			tempSchemaTable.Columns.Add(Expression);
			tempSchemaTable.Columns.Add(ColumnMapping);
			tempSchemaTable.Columns.Add(BaseTableNamespace);
			tempSchemaTable.Columns.Add(BaseColumnNamespace);
			return tempSchemaTable;
		}
	}
}