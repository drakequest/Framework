#if !NETCOREAPP

using System;
using Oracle.DataAccess.Client;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using DrakeQuest.Log;
using DrakeQuest.Configuration;
using DrakeQuest.Log.Generic;
using DrakeQuest.Data.Mapper;
using Autofac;
using DrakeQuest.Data.Oracle.Properties;
using DrakeQuest.DependencyInjection;

namespace DrakeQuest.Data.Oracle
{
	internal class OracleClient : DbDataProvider
	{
		#region Constructor

		public OracleClient(ILogger<OracleClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::Oracle.DataAccess.Client.OracleClientFactory.Instance, dbDataMapper) { }

		public OracleClient(ILogger<OracleClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, ConnectionContext connectionContext, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::Oracle.DataAccess.Client.OracleClientFactory.Instance, connectionContext, dbDataMapper) { }

		#endregion

		public override DbCommand CreateCommand()
		{
			OracleCommand comand = (OracleCommand)base.CreateCommand();
			comand.BindByName = true;
			return comand;
		}

		public override IDbCommand CreateCommand(IDbConnection con)
		{
			OracleCommand comand = (OracleCommand)base.CreateCommand(con);
			comand.BindByName = true;
			return comand;
		}

		public override IDataParameter CreateParameter(string parameterName, object parameterValue)
		{
			if (string.Compare(parameterName, Constants.OracleCursorParameterName, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				OracleParameter oraParameter = new OracleParameter(parameterName, OracleDbType.RefCursor);
				return oraParameter;
			}
			else
			{
				return new OracleParameter(parameterName, parameterValue);
			}
		}

		protected override IDataParameter[] PrepareParamters(IDataParameter[] dataParameters, bool isNonQuery = false, bool usePlainTextHasCommand = false)
		{
			if (usePlainTextHasCommand || isNonQuery)
				return dataParameters;

			var va = dataParameters.FirstOrDefault(o => o.ParameterName == Constants.OracleCursorParameterName);
			if (va == null)
			{
				OracleParameter oraParameter = new OracleParameter(Constants.OracleCursorParameterName, OracleDbType.RefCursor);
				List<IDataParameter> tmp = new List<IDataParameter>(dataParameters);
				oraParameter.Direction = ParameterDirection.Output;
				tmp.Add(oraParameter);
				dataParameters = tmp.ToArray();
			}
			return dataParameters;
		}

		public void SetDatetimeFormat(IDbConnection connection)
		{
			ExecuteNonQuery(connection, Constants.OracleSetSessionDateStatment);
		}

		public override void ExecuteBulkCopy(IDataReader reader, string destinationTable, IList<KeyValuePair<string, string>> mappings)
		{
			if (reader == null)
			{
				Logger.Log(LevelEnum.Error, Resources.ERR_DBREADER_ISNULL);
				throw new ArgumentNullException(nameof(reader), Resources.ERR_DBREADER_ISNULL);
			}

			if (string.IsNullOrWhiteSpace(destinationTable))
			{
				Logger.Log(LevelEnum.Error, Resources.ERR_TABLENAME_ISNULL);
				throw new ArgumentNullException(nameof(destinationTable), Resources.ERR_TABLENAME_ISNULL);
			}

			Logger.LogFormat(LevelEnum.Verbose, Resources.VERBOSE_BULK_INSERT, destinationTable);
			IConfigurationProvider config = Bootstrapper.GetContainer().Resolve<IConfigurationProvider>();
			using (OracleConnection destinationConnection = (OracleConnection)CreateConnection())
			{
				destinationConnection.Open();
				using (OracleBulkCopy bulkCopy = new OracleBulkCopy(destinationConnection))
				{
					bulkCopy.DestinationTableName = destinationTable;
					bulkCopy.BatchSize = Convert.ToInt32(config.GetAppSettings(DrakeQuest.Configuration.Constants.ConfigBulkCopyBatchSize));
					bulkCopy.BulkCopyTimeout = Convert.ToInt32(config.GetAppSettings(DrakeQuest.Configuration.Constants.ConfigBulkCopyTimeout));
					if (mappings != null)
						foreach (var mapping in mappings)
							bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
					bulkCopy.WriteToServer(reader);
					//bulkCopy.OracleRowsCopied += new OracleRowsCopiedEventHandler(bulkCopy_OracleRowsCopied);
					bulkCopy.Close();
					bulkCopy.Dispose();
					destinationConnection.Dispose();
				}
			}
			Logger.LogFormat(LevelEnum.Verbose, Resources.VERBOSE_BULK_INSERT_END, destinationTable);
		}

	}
}

#endif
