using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using DrakeQuest.Data.Mapper;
using DrakeQuest.Log.Generic;

namespace DrakeQuest.Data.Oracle
{
    internal class ManagedOracleClient : DbDataProvider
	{

         #region Constructor

         public ManagedOracleClient(ILogger<ManagedOracleClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance, dbDataMapper) { }

		public ManagedOracleClient(ILogger<ManagedOracleClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, ConnectionContext connectionContext, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance, connectionContext, dbDataMapper) { }

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
				oraParameter.Direction = ParameterDirection.Output;
				List<IDataParameter> tmp = new List<IDataParameter>(dataParameters);
				tmp.Add(oraParameter);
				dataParameters = tmp.ToArray();
			}
			return dataParameters;
		}

		public void SetDatetimeFormat(IDbConnection connection)
		{
			ExecuteNonQuery(connection, Constants.OracleSetSessionDateStatment);
		}

		public override IDataAdapter GetDataAdapter(IDbCommand command)
		{
			OracleDataAdapter oDataAdapter = new OracleDataAdapter();
			oDataAdapter.SelectCommand = (OracleCommand)command;
			return oDataAdapter;
		}
	}
}
