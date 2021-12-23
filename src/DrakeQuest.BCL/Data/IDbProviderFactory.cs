using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace DrakeQuest.Data
{
    public interface IDbProviderFactory
	{
		// Summary:
		//     Specifies whether the specific System.Data.Common.DbProviderFactory supports
		//     the System.Data.Common.DbDataSourceEnumerator class.
		//
		// Returns:
		//     true if the instance of the System.Data.Common.DbProviderFactory supports
		//     the System.Data.Common.DbDataSourceEnumerator class; otherwise false.
		bool CanCreateDataSourceEnumerator { get; }

		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbCommand
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbCommand.
		DbCommand CreateCommand();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbCommandBuilder
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbCommandBuilder.
		DbCommandBuilder CreateCommandBuilder();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbConnection
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbConnection.
		DbConnection CreateConnection();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbConnectionStringBuilder
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbConnectionStringBuilder.
		DbConnectionStringBuilder CreateConnectionStringBuilder();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbDataAdapter
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbDataAdapter.
		DbDataAdapter CreateDataAdapter();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbDataSourceEnumerator
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbDataSourceEnumerator.
		DbDataSourceEnumerator CreateDataSourceEnumerator();

		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the System.Data.Common.DbParameter
		//     class.
		//
		// Returns:
		//     A new instance of System.Data.Common.DbParameter
		DbParameter CreateParameter();

#if !NETCOREAPP
		//
		// Summary:
		//     Returns a new instance of the provider's class that implements the provider's
		//     version of the System.Security.CodeAccessPermission class.
		//
		// Parameters:
		//   state:
		//     One of the System.Security.Permissions.PermissionState values.
		//
		// Returns:
		//     A System.Security.CodeAccessPermission object for the specified System.Security.Permissions.PermissionState.
		CodeAccessPermission CreatePermission(PermissionState state);
#endif
	}

}
