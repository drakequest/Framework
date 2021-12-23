namespace DrakeQuest.Data
{
	/// <summary>
	/// List of provider enum that have existing implementations, still need to be registered
	/// </summary>
	public enum DatabaseProviderEnum
	{
		/// <summary>
		/// Use this one for registering Microsoft SQL Server Client providers
		/// </summary>
		SqlServerClient,
		/// <summary>
		/// Use this one for registering Oracle Managed  Client provider
		/// </summary>
		ManagedOracleClient,

		/// <summary>
		/// Use this one for registering Oracle Client provider, different from managed
		/// </summary>
		OracleClient,

		/// <summary>
		/// Use this one for registering legacy Microsoft Oracle Client provider
		/// </summary>
		MicrosoftOracle,

		/// <summary>
		/// Use this one for registering DB2 Client provider
		/// </summary>
		DB2
	}
}
