
namespace DrakeQuest.Configuration
{
	internal class ConnectionProvider : IConnectionProvider
	{

		private ConnectionStringElement GetConfiguration(string environmentName, string connectionName)
		{
			var environment = EnvironmentManager.Environments[environmentName];
			if (environment == null)
				throw new System.Configuration.ConfigurationErrorsException($"There is no enviromnemt {environmentName} configured.");

			var con = environment.ConnectionStrings[connectionName];
			if (con == null)
				throw new System.Configuration.ConfigurationErrorsException($"There is no connection {connectionName} configured in the environment {environmentName}.");

			return con;
		}

		public string GetConnectionString(string environmentName, string connectionName)
		{
			var con = GetConfiguration(environmentName, connectionName);
			return con.ConnectionString;
		}

		public string GetProviderName(string environmentName, string connectionName)
		{
			var con = GetConfiguration(environmentName, connectionName);
			return con.Provider;
		}
	}
}
