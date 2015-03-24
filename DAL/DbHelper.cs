using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
    public sealed class DbHelper<T> where T : DbConnection, new()
    {
        private readonly T _connection;

        public DbHelper(string connectionstring)
        {
            this._connection = new T {ConnectionString = connectionstring};
        }

        private DbConnection Connection
        {
            get { return _connection; }
        }

        private void PrepareCommand(IDbCommand command, IEnumerable<IDbDataParameter> parameters)
        {
            if (command == null || parameters == null)
                return;
            command.Parameters.Clear();
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        public int ExecuteNoneQuery(CommandType commandType, string commandText, IDbDataParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            if (!Equals(parameters, null) && parameters.Length == 0)
            {
                throw new ArgumentNullException("parameters");
            }

            using (var command = this.Connection.CreateCommand())
            {
                this.PrepareCommand(command, parameters);
                command.CommandType = commandType;
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }

        }

        public int ExecuteStoredProcedure(string spName, params IDbDataParameter[] parameters)
        {
            return this.ExecuteNoneQuery(CommandType.StoredProcedure,spName, parameters);
        }

        public int ExecuteSql(string sql, IDbDataParameter[] parameters = null)
        {
            return this.ExecuteNoneQuery(CommandType.Text, sql, parameters);
        }

        public void ExecuteSqlByTran(IDictionary<string, IDbDataParameter[]> dics)
        {
            using (IDbTransaction tran = Connection.BeginTransaction())
            {
                try
                {
                    foreach (var dic in dics)
                    {
                        this.ExecuteSql(dic.Key, dic.Value);
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

        public DataSet ExecuteDataSet<TU>(CommandType commandType, string commandText, IDbDataParameter[] parameters) where TU:DbDataAdapter,new()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;
                this.PrepareCommand(command, parameters);
                using (var da = new TU())
                {
                    da.SelectCommand = command;
                    using (var ds = new DataSet())
                    {
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText, IDbDataParameter[] parameters)
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;
                this.PrepareCommand(command, parameters);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public IDataReader ExecuteReader(string sql, IDbDataParameter[] parameters)
        {
            return this.ExecuteReader(CommandType.Text, sql, parameters);
        }

        public IDataReader ExecuteStoreProcudeReader(string spName, string commandText, IDbDataParameter[] parameters)
        {
            return this.ExecuteReader(CommandType.StoredProcedure, spName, parameters);
        }

    }
}