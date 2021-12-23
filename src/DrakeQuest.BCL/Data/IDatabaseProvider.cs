using System;
using System.Collections.Generic;
using System.Data;

namespace DrakeQuest.Data
{
	/// <summary>
	/// Data Access Layer generique 
	/// </summary>
	public interface IDatabaseProvider : IDisposable
	{
		/// <summary>
		/// Connection Informations for the database (name, provider, etc...)
		/// </summary>
		ConnectionContext ConnectionContext { get; }

		/// <summary>
		/// Generate connection string from system
		/// </summary>
		/// <returns></returns>
		IDbConnection CreateConnection();

		/// <summary>
		/// Generate connection string from system
		/// </summary>
		/// <returns></returns>
		IDbCommand CreateCommand(IDbConnection con);

		/// <summary>
		/// Create parameter from command
		/// </summary>
		/// <param name="parameterName">name of the parameter</param>
		/// <param name="parameterValue">value for the parameter</param>
		/// <returns></returns>
		IDataParameter CreateParameter(string parameterName, object parameterValue);

		#region query

		/// <summary>
		/// Execute non query statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns> 
		int ExecuteNonQuery(string statment, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		object ExecuteScalar(string statment, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		T ExecuteScalar<T>(string statment, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Reader with fill action to specify
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="fillAction"></param>
		/// <param name="dataParameters"></param>
		void ExecuteReader(string statment, Action<IDataReader> fillAction, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		IEnumerable<T> ExecuteReader<T>(object mapperKey, string statment, params IDataParameter[] dataParameters)
			where T : new();

		void ExecuteBulkCopy(IDataReader reader, string destinationTable, IList<KeyValuePair<string, string>> mappings);

		#endregion

		DataTable Fill(DataTable data, string statment, bool usePlainTextHasCommand = true, params IDataParameter[] dataParameters);

		#region Use existing IDbConnection

		int ExecuteNonQuery(IDbConnection con, string statment, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		object ExecuteScalar(IDbConnection con, string statment, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Reader with fill action to specify
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="fillAction"></param>
		/// <param name="dataParameters"></param>
		void ExecuteReader(IDbConnection con, string statment, Action<IDataReader> fillAction, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		T ExecuteScalar<T>(IDbConnection con, string statment, params IDataParameter[] dataParameters);

		/// <summary>
		/// Execute Sclar statment
		/// </summary>
		/// <param name="statment"></param>
		/// <param name="dataParameters"></param>
		/// <returns></returns>
		IEnumerable<T> ExecuteReader<T>(object mapperKey, IDbConnection con, string statment, params IDataParameter[] dataParameters)
					where T : new();

		DataTable Fill(DataTable dataTable, IDbConnection con, string statment, bool usePlainTextHasCommand = true, params IDataParameter[] dataParameters);

		#endregion

		int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		int Execute(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		IEnumerable<T> Query<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

		IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

		T QueryFirst<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		T QueryFirst<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		T QueryFirstOrDefault<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
	}

}
