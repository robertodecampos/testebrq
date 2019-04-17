using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Usuario : IDisposable
    {
        public int Id { get; set; }
        public string Login { get; set; }

        public bool EstaValido(out List<string> mensagens)
        {
            mensagens = new List<string>();

            if (string.IsNullOrEmpty(Login))
                mensagens.Add("O campo Nome é obrigatório!");
            else if (Login.Trim().Length <= 2)
                mensagens.Add("O Login deve conter ao menos 3 caracteres");

            return (mensagens.Count == 0);
        }

        public bool EstaValido(out string mensagem)
        {
            mensagem = string.Empty;

            List<string> mensagens;

            if (EstaValido(out mensagens))
                return true;
            else
            {
                mensagem = mensagens[0];
                return false;
            }
        }

        public bool EstaValido()
        {
            List<string> mensagens;

            return EstaValido(out mensagens);
        }

        public void Dispose() { }
    }
}
