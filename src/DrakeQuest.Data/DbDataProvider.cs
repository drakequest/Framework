using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;
using DrakeQuest.Data.Mapper;
using System.Text;
using DrakeQuest.Data.Properties;

namespace DrakeQuest.Data
{
	public abstract class DbDataProvider : DbProviderFactory, IDbProviderFactory, IDatabaseProvider
	{
		#region Properties

		public const string LoggerName = "[DatabaseAccess]";

		public const int DefaultCommandTimeout = 120;

        public ConnectionContext ConnectionContext { get; }

		public static int CommandTimeout
		{
			get
			{
				int commandTimeout = 0;
				var configValue = ConfigurationManager.AppSettings["commandTimeout"];
				if (int.TryParse(configValue, out commandTimeout))
					return commandTimeout;
				return DefaultCommandTimeout;
			}
		}

		protected DbProviderFactory Factory { get; private set; }

		protected ILogger Logger { get; private set; }

		protected Lazy<IDataReaderMapperFactory> Mapper { get; private set; }

		protected Lazy<IDbDataMapper> DbDataMapper { get; private set; }

		#endregion

		#region Constructor & Destructor

		/// <summary>
		/// Create the database provider, with the logger, and the Db provider factory 
		/// </summary>
		/// <param name="logger">Logger used for this database provider</param>
		/// <param name="factory">DB Factory that will be used as Factory for conenction/parameters</param>
		protected DbDataProvider(ILogger<DbDataProvider> logger, Lazy<IDataReaderMapperFactory> mapperFactory, DbProviderFactory factory, Lazy<IDbDataMapper> dbDataMapper)
		{
			if (factory == null)
				throw new ArgumentNullException("factory", Resources.ERR_CONNECTION_NOT_PROVIDERNAME_CONFIGURED);

			if (logger == null)
				throw new ArgumentNullException("logger", Resources.ERR_LOGGER_NOT_CONFIGURED);

			Mapper = mapperFactory;
			Factory = factory;
			Logger = logger;
			DbDataMapper = dbDataMapper;
		}

		/// <summary>
		/// Create the database provider, with the logger, and the Db provider factory 
		/// </summary>
		/// <param name="logger">Logger used for this database provider</param>
		/// <param name="factory">DB Factory that will be used as Factory for conenction/parameters</param>
		/// <param name="connectionString">Connection string used for this database</param>
		protected DbDataProvider(ILogger<DbDataProvider> logger, Lazy<IDataReaderMapperFactory> mapperFactory, DbProviderFactory factory, ConnectionContext connectionContext, Lazy<IDbDataMapper> dbDataMapper)
			: this(logger, mapperFactory, factory, dbDataMapper)
		{
			if (connectionContext == null || string.IsNullOrWhiteSpace(connectionContext.ConnectionString))
				throw new ArgumentNullException("connectionContext", Resources.ERR_CONNECTION_NOT_DEFINED);

            ConnectionContext = connectionContext;
        }

		/// <summary>
		/// Destructor, call the Dispose.
		/// </summary>
		~DbDataProvider()
		{
			this.Dispose();
		}

		public virtual void Dispose() { }

		#endregion

		#region Methods

		#region DbProviderFactory Implementation

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnection"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbConnection"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbConnection CreateConnection()
		{
			DbConnection con = Factory.CreateConnection();
			con.ConnectionString = this.ConnectionContext.ConnectionString;
			return con;
		}

		/// <summary>
		/// Specifies whether the specific <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
		/// </summary>
		/// <returns>
		/// true if the instance of the <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class; otherwise false.
		/// </returns>
		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				return Factory.CanCreateDataSourceEnumerator;
			}
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbCommand"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbCommand"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbCommand CreateCommand()
		{
			DbCommand command = Factory.CreateCommand();
			return command;
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbCommandBuilder"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbCommandBuilder"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return Factory.CreateCommandBuilder();
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnectionStringBuilder"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return Factory.CreateConnectionStringBuilder();
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataAdapter"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbDataAdapter"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbDataAdapter CreateDataAdapter()
		{
			return Factory.CreateDataAdapter();
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbDataSourceEnumerator"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return Factory.CreateDataSourceEnumerator();
		}

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbParameter"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbParameter"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override DbParameter CreateParameter()
		{
			return Factory.CreateParameter();
		}

#if !NETCOREAPP

		/// <summary>
		/// Returns a new instance of the provider's class that implements the provider's version of the <see cref="T:System.Security.CodeAccessPermission"/> class.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Security.CodeAccessPermission"/> object for the specified <see cref="T:System.Security.Permissions.PermissionState"/>.
		/// </returns>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState"/> values.</param><filterpriority>2</filterpriority>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return Factory.CreatePermission(state);
		}
#endif

		#endregion

		#region database connectivity

		/// <summary>
		/// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnection"/> class.
		/// </summary>
		/// <returns>
		/// A new instance of <see cref="T:System.Data.Common.DbConnection"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IDbConnection IDatabaseProvider.CreateConnection()
		{
			return this.CreateConnection();
		}

		private void OpenConnection(IDbConnection con)
		{
			try
			{
				if (con.State == ConnectionState.Closed)
					con.Open();
			}
			catch (Exception ex)
			{
				Logger.Log(LevelEnum.Error,ex,  Resources.ERR_CONNECTION_CANTOPEN);
				DataException dataException = new DataException( Resources.ERR_CONNECTION_CANTOPEN, ex);
				throw dataException;
			}
		}

		private void PopulateParameterImpl(IDbCommand command, bool isNonQuery , bool usePlainText, IDataParameter[] dataParameters)
		{
			if (dataParameters != null && dataParameters.Length > 0)
			{
				dataParameters = PrepareParamters(dataParameters, isNonQuery, usePlainText);
				foreach (var param in dataParameters)
				{
					command.Parameters.Add(param);
				}
			}
		}

		protected virtual IDataParameter[] PrepareParamters(IDataParameter[] dataParameters, bool isNonQuery, bool usePlainText)
		{
			return dataParameters;
		}

		public virtual IDbCommand CreateCommand(IDbConnection con)
		{
			var command = con.CreateCommand();
			command.CommandTimeout = CommandTimeout;
			return command;
		}

		public abstract IDataParameter CreateParameter(string parameterName, object parameterValue);

		public virtual void ConfigureReader(IDataReader reader) { }

#endregion

#region Action
		/// <summary>
		///  ds
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="connection"></param>
		/// <param name="usePlainTextHasCommand"></param>
		/// <returns></returns>
		private IDbCommand CreateCommandImpl(string commandText, IDbConnection connection, bool usePlainTextHasCommand = false)
		{
			if (string.IsNullOrWhiteSpace(commandText))
			{
				Logger.Log(LevelEnum.Error, Resources.ERR_STATMENT_MISSING);
				throw new ArgumentNullException("commandText", Resources.ERR_STATMENT_MISSING);
			}

			IDbCommand command = CreateCommand(connection);
			command.CommandText = commandText;
			command.CommandType = usePlainTextHasCommand ? CommandType.Text : CommandType.StoredProcedure;
			return command;
		}

#region base Idbcommand wrapper

        private string ToString(IDataParameter[] parameters)
        {
            if (parameters == null)
                return null;
            if (parameters.Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (IDataParameter parameter in parameters)
                sb.AppendLine($"\t'{parameter.ParameterName}': '{parameter.Value}',");
            sb.AppendLine("}");
            return sb.ToString();
        }

		private void ExecuteReaderImpl(IDbConnection con, IDbCommand command, Action<IDataReader> fillAction, bool usePlainTextHasCommand, params IDataParameter[] dataParameters)
		{
			Logger.LogFormat(LevelEnum.Verbose,Resources.INFO_EXECUTE_READER, command.CommandText);
			if (usePlainTextHasCommand)
			{
				Logger.LogFormat(LevelEnum.Alert,Resources.INFO_EXECUTE_RUNNING_PLAINSQL);
			}
			IDataReader reader = null;
			try
			{
				OpenConnection(con);
				PopulateParameterImpl(command, false,usePlainTextHasCommand, dataParameters);
				reader = command.ExecuteReader();
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, command.CommandText);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				ex.Data["CommandText"] = command.CommandText;
				ex.Data["ComandTimeout"] = command.CommandTimeout;
				ex.Data["Database"] = con.Database;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}

			try
			{
				fillAction(reader);
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, command.CommandText);
				Logger.Log(LevelEnum.Error, ex, msg);
				DataException dataException = new DataException(msg, ex);
				ex.Data["CommandText"] = command.CommandText;
				ex.Data["ComandTimeout"] = command.CommandTimeout;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;

			}
			Logger.LogFormat(LevelEnum.Verbose, Resources.INFO_EXECUTE_READER_END, command.CommandText);
		}

		private int ExecuteNonQueryImpl(IDbConnection con, IDbCommand command, bool usePlainTextHasCommand, params IDataParameter[] dataParameters)
		{
			Logger.LogFormat(LevelEnum.Verbose,Resources.INFO_EXECUTE_NONQuery, command.CommandText);
			if (usePlainTextHasCommand)
			{
				Logger.LogFormat(LevelEnum.Alert,Resources.INFO_EXECUTE_RUNNING_PLAINSQL);
			}
			int result = 0;
			try
			{
				OpenConnection(con);
				PopulateParameterImpl(command, true, usePlainTextHasCommand, dataParameters);
				result = command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, command.CommandText);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
                ex.Data["CommandText"] = command.CommandText;
                ex.Data["ComandTimeout"] = command.CommandTimeout;
                ex.Data["Database"] = con.Database;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}
			Logger.LogFormat(LevelEnum.Verbose, Resources.INFO_EXECUTE_NonQueryEnd, command.CommandText);
			return result;
		}

		private object ExecuteScalarImpl(IDbConnection con, IDbCommand command, bool usePlainTextHasCommand, params IDataParameter[] dataParameters)
		{
			Logger.LogFormat(LevelEnum.Verbose,Resources.INFO_EXECUTE_SCALAR, command.CommandText);
			if (usePlainTextHasCommand)
			{
				Logger.LogFormat(LevelEnum.Alert,Resources.INFO_EXECUTE_RUNNING_PLAINSQL);
			}
			object result = null;
			try
			{
				OpenConnection(con);
				PopulateParameterImpl(command, false, usePlainTextHasCommand, dataParameters);
				result = command.ExecuteScalar();

			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, command.CommandText);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
                ex.Data["CommandText"] = command.CommandText;
                ex.Data["ComandTimeout"] = command.CommandTimeout;
                ex.Data["Database"] = con.Database;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}
			Logger.LogFormat(LevelEnum.Verbose, Resources.INFO_EXECUTE_SCALAR_END, command.CommandText);
			return result;
		}

#endregion

#region Execute reader

		public void ExecuteReader(string statment, Action<IDataReader> fillAction, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters)
		{
			if (fillAction == null)
			{
				Logger.Log(LevelEnum.Warn,  Resources.ERR_READER_FILLACTION);
				throw new ArgumentNullException( Resources.ERR_READER_FILLACTION);
			}

			try
			{
				using (IDbConnection con = CreateConnection())
				using (IDbCommand command = CreateCommandImpl(statment, con))
				{
					ExecuteReaderImpl(con, command, fillAction, usePlainTextHasCommand, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format(  Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}
		}

		public void ExecuteReader(IDbConnection connection, string statment, Action<IDataReader> fillAction, params IDataParameter[] dataParameters)
		{
			if (connection == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_CONNECTION_NULL);
				throw new ArgumentNullException("connection", Resources.ERR_CONNECTION_NULL);
			}

			if (fillAction == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_READER_FILLACTION);
				throw new ArgumentNullException("fillAction", Resources.ERR_READER_FILLACTION);
			}


			try
			{
				using (IDbCommand command = CreateCommandImpl(statment, connection))
				{
					ExecuteReaderImpl(connection, command, fillAction, false, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}


		}

		public IEnumerable<T> ExecuteReader<T>(object mapperKey, string statment, params IDataParameter[] dataParameters)
					where T : new()
		{
			IDbConnection con = null;
			try
			{
				con = CreateConnection();
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}

			foreach (var _value in ExecuteReader<T>(mapperKey, con, statment, dataParameters))
			{
				yield return _value;
			}

			try
			{
				using (con) { }
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}

		}

		public IEnumerable<T> ExecuteReader<T>(object mapperKey, IDbConnection connection, string statment, params IDataParameter[] dataParameters)
			where T: new()
		{
			IDataReaderObjectMapper mapper = null;
			if (mapperKey == null)
				mapperKey = statment;

			IDbCommand command = null;
			IDataReader reader = null;
			try
			{
				command = CreateCommandImpl(statment, connection);

				OpenConnection(connection);
				PopulateParameterImpl(command, false, false, dataParameters);
				Logger.LogFormat(LevelEnum.Verbose,Resources.INFO_EXECUTE_READER, command.CommandText);
				reader = command.ExecuteReader();
				mapper = Mapper.Value.GetMapper<T>(mapperKey, reader);
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, command.CommandText);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
                ex.Data["CommandText"] = command.CommandText;
                ex.Data["ComandTimeout"] = command.CommandTimeout;
                ex.Data["Database"] = connection.Database;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}

			while (reader.Read())
			{
				T data = new T();
				mapper.Populate(ref data, reader);
				yield return data;
			}

			Logger.LogFormat(LevelEnum.Verbose, Resources.INFO_EXECUTE_READER_END, command.CommandText);
			try
			{
				using (command) { }
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
                ex.Data["CommandText"] = command.CommandText;
                ex.Data["ComandTimeout"] = command.CommandTimeout;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}
		}

#endregion

#region Execute non query

		public int ExecuteNonQuery(string statment, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters)
		{
			try
			{
				using (IDbConnection con = CreateConnection())
				using (IDbCommand command = CreateCommandImpl(statment, con, usePlainTextHasCommand))
				{
					return ExecuteNonQueryImpl(con, command, usePlainTextHasCommand, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format( Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}
		}

		public int ExecuteNonQuery(IDbConnection connection, string statment, params IDataParameter[] dataParameters)
		{
			if (connection == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_CONNECTION_NULL);
				throw new ArgumentNullException("connection", Resources.ERR_CONNECTION_NULL);
			}

			try
			{
				using (IDbCommand command = CreateCommandImpl(statment, connection))
				{
					return ExecuteNonQueryImpl(connection, command, false, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format( Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}
		}

#endregion

#region Execute Scalar

		public object ExecuteScalar(string statment, bool usePlainTextHasCommand = false, params IDataParameter[] dataParameters)
		{
			try
			{
				using (IDbConnection con = CreateConnection())
				using (IDbCommand command = CreateCommandImpl(statment, con, usePlainTextHasCommand))
				{
					return ExecuteScalarImpl(con, command, usePlainTextHasCommand, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format( Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
				throw dataException;
			}
		}

		public object ExecuteScalar(IDbConnection connection, string statment, params IDataParameter[] dataParameters)
		{

			if (connection == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_CONNECTION_NULL);
				throw new ArgumentNullException("connection", Resources.ERR_CONNECTION_NULL);
			}
			try
			{
				using (IDbCommand command = CreateCommandImpl(statment, connection))
				{
					return ExecuteScalarImpl(connection, command, false ,dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string msg = string.Format(Resources.ERR_EXECUTING_EXECUTE, statment);
				Logger.Log(LevelEnum.Error,ex, msg);
				DataException dataException = new DataException(msg, ex);
                ex.Data["CommandText"] = statment;
                ex.Data["Database"] = connection.Database;
                ex.Data["Parameters"] = ToString(dataParameters);
                throw dataException;
			}
		}

		public T ExecuteScalar<T>(string statment, params IDataParameter[] dataParameters)
		{
			object result = ExecuteScalar(statment, dataParameters:dataParameters);
			return (T)Convert.ChangeType(result, typeof(T));
		}

		public T ExecuteScalar<T>(IDbConnection con, string statment, params IDataParameter[] dataParameters)
		{
			object result = ExecuteScalar(con, statment, dataParameters);
			return (T)Convert.ChangeType(result, typeof(T));

		}

#endregion

		public virtual IDataAdapter GetDataAdapter(IDbCommand command)
		{
			var dbAdpter = Factory.CreateDataAdapter();
			dbAdpter.SelectCommand = (DbCommand)command;
			return dbAdpter;
		}

		private DataTable FillImpl(DataTable dataTable, IDbConnection con, IDbCommand command, params IDataParameter[] dataParameters)
		{
			Logger.LogFormat(LevelEnum.Verbose,Resources.INFO_FILL, command.CommandText);
			try
			{
				if (dataTable == null)
					dataTable = new DataTable();
				ExecuteReaderImpl(con, command, (reader) => dataTable.Load(reader), false ,dataParameters);
			}
            catch (DataException dataEx)
            {
                throw;
            }
            catch (Exception ex)
			{
				Logger.Log(LevelEnum.Error,ex, ex.Message);
				throw new DataException(ex.Message, ex);
			}
			Logger.LogFormat(LevelEnum.Verbose, Resources.INFO_FILL_END, command.CommandText);
			return dataTable;
		}

		public DataTable Fill(DataTable dataTable, IDbConnection con, IDbCommand command, params IDataParameter[] dataParameters)
		{
			if (con == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_CONNECTION_NULL);
				throw new ArgumentNullException(Resources.ERR_CONNECTION_NULL);
			}


			if (command == null)
			{
				Logger.Log(LevelEnum.Warn,Resources.ERR_COMMAND_NULL);
				throw new ArgumentNullException(Resources.ERR_COMMAND_NULL);
			}

			try
			{
				return FillImpl(dataTable, con, command, dataParameters);
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				Logger.Log(LevelEnum.Error,ex, ex.Message);
				throw new DataException(ex.Message, ex);
			}
		}

		public DataTable Fill(DataTable dataTable, string statment, bool usePlainTextHasCommand = true, params IDataParameter[] dataParameters)
		{
			try
			{
				using (IDbConnection con = CreateConnection())
				using (IDbCommand command = CreateCommandImpl(statment, con, usePlainTextHasCommand : usePlainTextHasCommand))
				{
					return FillImpl(dataTable, con, command, dataParameters);
				}
			}
			catch (DataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				Logger.Log(LevelEnum.Error,ex, ex.Message);
				throw new DataException(ex.Message, ex);
			}

		}

        public DataTable Fill(DataTable dataTable, IDbConnection con,  string statment, bool usePlainTextHasCommand = true, params IDataParameter[] dataParameters)
        {
            if (con == null)
            {
                Logger.Log(LevelEnum.Warn, Resources.ERR_CONNECTION_NULL);
                throw new ArgumentNullException(Resources.ERR_CONNECTION_NULL);
            }

            try
            {
                using (IDbCommand command = CreateCommandImpl(statment, con, usePlainTextHasCommand: usePlainTextHasCommand))
                {
                    return FillImpl(dataTable, con, command, dataParameters);
                }
            }
            catch (DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log(LevelEnum.Error, ex, ex.Message);
                throw new DataException(ex.Message, ex);
            }

        }

		#endregion

		#region Mapper Query

		public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.Execute(CreateConnection(), sql, param, transaction, commandTimeout, commandType);
		}

		public int Execute(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.Execute(con, sql, param, transaction, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.Query<T>(con, sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.Query<T>(CreateConnection(), sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public T QueryFirst<T>( string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.QueryFirst<T>(CreateConnection(), sql, param, transaction, commandTimeout, commandType);
		}

		public T QueryFirst<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.QueryFirst<T>(con, sql, param, transaction, commandTimeout, commandType);
		}

		public T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.QueryFirstOrDefault<T>(CreateConnection(), sql, param, transaction, commandTimeout, commandType);
		}

		public T QueryFirstOrDefault<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return DbDataMapper.Value.QueryFirstOrDefault<T>(con, sql, param, transaction, commandTimeout, commandType);
		}

		#endregion

		#endregion

		public virtual void ExecuteBulkCopy(IDataReader reader, string destinationTable, IList<KeyValuePair<string, string>> mappings)
		{
			throw new NotImplementedException();
		}

	}
}
