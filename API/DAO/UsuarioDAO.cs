using API.Models;
using API.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API.DAO
{
    public class UsuarioDAO : IDisposable
    {
        private Conexao _connection;

        public UsuarioDAO(Conexao _connection) => this._connection = _connection;

        public bool VerificarLoginESenha(Usuario usuario, string senha, MySqlTransaction transaction)
        {
            if (string.IsNullOrEmpty(usuario.Login))
                throw new Exception($"É necessário que o usuário possua a propriedade {nameof(usuario.Login)} preenchida!");

            string sql = " SELECT *               " +
                         " FROM usuario a         " +
                         " WHERE a.login = @login " +
                         "   AND a.senha = @senha ";

            var parametros = new List<MySqlParameter>(2);
            parametros.Add(new MySqlParameter("@login", MySqlDbType.String) { Value = usuario.Login });
            parametros.Add(new MySqlParameter("@senha", MySqlDbType.String) { Value = senha });

            using (DataTable dt = _connection.ExecuteReader(sql, parametros, transaction))
            {
                if (dt.Rows.Count == 0)
                    return false;
                else if (dt.Rows.Count > 1)
                    throw new Exception("Ocorreu um problema na verificação do usuário! Existe mais de 1 usuário com o mesmo login e mesma senha!");

                PreencherModel(usuario, dt.Rows[0]);

                return true;
            }
        }

        private void PreencherModel(Usuario usuario, DataRow registro)
        {
            usuario.Id = int.Parse(registro["id"].ToString());
            usuario.Login = registro["login"].ToString();
        }

        public void Dispose() { }
    }
}
