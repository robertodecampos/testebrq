using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class LogTentativaLogin : IDisposable
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public DateTime DataTentativa { get; set; }
        public bool LoginEfetuado { get; set; }

        public void Dispose() { }
    }
}
