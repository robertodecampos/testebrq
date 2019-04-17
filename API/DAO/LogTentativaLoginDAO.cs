using API.Models;
using API.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace API.DAO
{
    public class LogTentativaLoginDAO : IDisposable
    {
        private Conexao conexao;

        public LogTentativaLoginDAO(Conexao conexao) => this.conexao = conexao;

        public void Cadastrar(LogTentativaLogin logTentativaLogin, MySqlTransaction transaction)
        {
            string sql = " INSERT INTO log_tentativa_login " +
                         "   (login, dataTentativa)        " +
                         " VALUES                          " +
                         "   (@login, NOW())               ";

            var parametros = new List<MySqlParameter>(1);
            parametros.Add(new MySqlParameter("@login", MySqlDbType.String) { Value = logTentativaLogin.Login });

            conexao.Execute(sql, parametros, transaction);

            sql = " SELECT *                   " +
                  " FROM log_tentativa_login a " +
                  " WHERE a.id = @id           ";

            parametros.Clear();
            parametros.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = conexao.UltimoIdInserido() });

            using (DataTable dt = conexao.ExecuteReader(sql, parametros, transaction))
            {
                PreencherModel(logTentativaLogin, dt.Rows[0]);
            }
        }

        public void MarcarComoLoginEfetuado(LogTentativaLogin logTentativaLogin, MySqlTransaction transaction)
        {
            if (logTentativaLogin.Id == 0)
                throw new Exception($"Não é possível marcar um log como login efetuado sem o {nameof(logTentativaLogin.Id)}!");

            string sql = " UPDATE log_tentativa_login SET " +
                         "   loginEfetuado = 1            " +
                         " WHERE id = @id                 ";

            var parametros = new List<MySqlParameter>(1);
            parametros.Add(new MySqlParameter("@id", MySqlDbType.Int32) { Value = logTentativaLogin.Id });

            conexao.Execute(sql, parametros, transaction);
        }

        public IList<LogTentativaLogin> ListarPeloLogin(string login, MySqlTransaction transaction)
        {
            string sql = " SELECT *                   " +
                         " FROM log_tentativa_login a " +
                         " WHERE a.login = @login     ";

            var parametros = new List<MySqlParameter>(1);
            parametros.Add(new MySqlParameter("@login", MySqlDbType.String) { Value = login });

            using (DataTable dt = conexao.ExecuteReader(sql, parametros))
            {
                var logsTentativaLogin = new List<LogTentativaLogin>(dt.Rows.Count);

                foreach (DataRow dr in dt.Rows)
                {
                    var logTentativaLogin = new LogTentativaLogin();
                    PreencherModel(logTentativaLogin, dr);
                    logsTentativaLogin.Add(logTentativaLogin);
                }

                return logsTentativaLogin;
            }
        }

        private void PreencherModel(LogTentativaLogin logTentativaLogin, DataRow registro)
        {
            logTentativaLogin.Id = int.Parse(registro["id"].ToString());
            logTentativaLogin.Login = registro["login"].ToString();
            logTentativaLogin.DataTentativa = DateTime.Parse(registro["dataTentativa"].ToString());
            logTentativaLogin.LoginEfetuado = bool.Parse(registro["loginEfetuado"].ToString());
        }

        public void Dispose() { }
    }
}
