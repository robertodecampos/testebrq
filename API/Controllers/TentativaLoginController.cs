using System;
using System.Linq;
using API.DAO;
using API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    public class TentativaLoginController : Controller
    {
        private Conexao conexao;

        public TentativaLoginController(Conexao conexao) => this.conexao = conexao;

        [HttpGet]
        [Route("listar")]
        public IActionResult Index()
        {
            try
            {
                string login = User.Claims.Where(c => c.Type == "login").ToArray()[0].Value;

                using (var logTentativaLoginDAO = new LogTentativaLoginDAO(conexao))
                {
                    return Ok(logTentativaLoginDAO.ListarPeloLogin(login, null));
                }
            } catch (Exception erro)
            {
#if DEBUG
                return StatusCode(500, erro);
#else
                return StatusCode(500);
#endif
            }
        }
    }
}