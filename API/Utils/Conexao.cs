using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace API.Utils
{
    public class Conexao : IDisposable
    {
        protected MySqlConnection _conn;

        public Conexao(IConfiguration configuration)
        {
            _conn = new MySqlConnection(configuration.GetConnectionString("Default"));
            _conn.Open();
        }

        private void SetParametersToCommand(MySqlCommand command, IList<MySqlParameter> parameters)
        {
            foreach (MySqlParameter parameter in parameters)
                command.Parameters.Add(parameter);
        }

        public int Execute(string sql, IList<MySqlParameter> parameters = null, MySqlTransaction transaction = null)
        {
            using (var command = new MySqlCommand(sql, _conn))
            {
                if (parameters != null)
                    SetParametersToCommand(command, parameters);

                if (transaction != null)
                    command.Transaction = transaction;

                return command.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteReader(string sql, IList<MySqlParameter> parameters = null, MySqlTransaction transaction = null)
        {
            using (var command = new MySqlCommand(sql, _conn))
            {
                if (parameters != null)
                    SetParametersToCommand(command, parameters);

                if (transaction != null)
                    command.Transaction = transaction;

                using (MySqlDataReader dr = command.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(dr);

                    dr.Close();

                    return dt;
                }
            }
        }

        public int UltimoIdInserido() => int.Parse(ExecuteReader("SELECT LAST_INSERT_ID() AS id").Rows[0]["id"].ToString());

        public MySqlTransaction BeginTransaction() => _conn.BeginTransaction();

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
