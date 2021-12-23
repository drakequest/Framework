using System;
using System.Data;
using System.Data.SqlClient;
using DrakeQuest.Data.Mapper;
using DrakeQuest.Log.Generic;

namespace DrakeQuest.Data.SqlServer
{
	internal class SqlServerClient : DbDataProvider
	{
        #region Constructor

        public SqlServerClient(ILogger<SqlServerClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::System.Data.SqlClient.SqlClientFactory.Instance, dbDataMapper) { }

		public SqlServerClient(ILogger<SqlServerClient> logger, Lazy<IDataReaderMapperFactory> mapperFactory, ConnectionContext connectionContext, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, global::System.Data.SqlClient.SqlClientFactory.Instance, connectionContext, dbDataMapper) { }

		#endregion

		public override IDataParameter CreateParameter(string parameterName, object parameterValue)
		{
			if (!parameterName.StartsWith("@"))
			{
				parameterName = '@' + parameterName;
			}
			return new SqlParameter(parameterName, parameterValue);
		}

		protected override IDataParameter[] PrepareParamters(IDataParameter[] dataParameters, bool isNonQuery = false, bool usePlainTextHasCommand = false)
		{
			return dataParameters;
		}
	}
}
