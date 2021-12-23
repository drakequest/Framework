namespace DrakeQuest.Configuration
{
	/// <summary>
	/// Connection provider interface
	/// </summary>
    public interface IConnectionProvider
	{
		/// <summary>
		/// Get the Connection string for an environment and connection name
		/// </summary>
		/// <param name="environmentName">Environment Name</param>
		/// <param name="connectionName">Connection Name</param>
		/// <returns>Connection string to use against the server</returns>
		string GetConnectionString(string environmentName, string connectionName);

		/// <summary>
		/// Get the Provider name for an environment and connection name
		/// </summary>
		/// <param name="environmentName">Environment Name</param>
		/// <param name="connectionName">Connection Name</param>
		/// <returns>Provider to use with the connection</returns>
		string GetProviderName(string environmentName, string connectionName);
	}
}
