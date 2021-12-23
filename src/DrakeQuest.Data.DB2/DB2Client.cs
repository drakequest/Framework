using System;
using System.Data;

using DrakeQuest.Data.Mapper;
using DrakeQuest.Log.Generic;

#if NETCOREAPP
using IBM.Data.DB2.Core;
using DB2Factory = global::IBM.Data.DB2.Core.DB2Factory;
#else
  using IBM.Data.DB2;
	using DB2Factory = global::IBM.Data.DB2.DB2Factory;
#endif




namespace DrakeQuest.Data.DB2
{
	internal class DB2Client : DbDataProvider
	{
        #region Constructor

        public DB2Client(ILogger<DB2Client> logger, Lazy<IDataReaderMapperFactory> mapperFactory, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, DB2Factory.Instance, dbDataMapper) { }

		public DB2Client(ILogger<DB2Client> logger, Lazy<IDataReaderMapperFactory> mapperFactory, ConnectionContext connectionContext, Lazy<IDbDataMapper> dbDataMapper)
			: base(logger, mapperFactory, DB2Factory.Instance, connectionContext, dbDataMapper) { }

		#endregion

		public override IDataParameter CreateParameter(string parameterName, object parameterValue)
		{
			if (!parameterName.StartsWith("@"))
			{
				parameterName = '@' + parameterName;
			}
			return new DB2Parameter(parameterName, parameterValue);
		}

		protected override IDataParameter[] PrepareParamters(IDataParameter[] dataParameters, bool isNonQuery = false, bool usePlainTextHasCommand = false)
		{
			return dataParameters;
		}
	}
}
