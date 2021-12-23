using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrakeQuest.Data.Mapper
{
	internal class DapperMapper : IDbDataMapper
	{
		public int Execute(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Dapper.SqlMapper.Execute(con, sql, param, transaction, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Dapper.SqlMapper.Query<T>(con, sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public T QueryFirst<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Dapper.SqlMapper.QueryFirst<T>(con, sql, param, transaction, commandTimeout, commandType);
		}

		public T QueryFirstOrDefault<T>(IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Dapper.SqlMapper.QueryFirstOrDefault<T>(con, sql, param, transaction, commandTimeout, commandType);
		}
	}
}
