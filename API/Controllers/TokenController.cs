using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.DAO;
using API.Models;
using API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class TokenController : Controller
    {
        private Conexao conexao;
        private IConfiguration configuracao;

        public TokenController(Conexao conexao, IConfiguration configuracao)
        {
            this.conexao = conexao;
            this.configuracao = configuracao;
        }

        [HttpGet]
        [Route("gerar")]
        public IActionResult Gerar(string login, string senha)
        {
            try
            {
                using (var usuario = new Usuario())
                using (var usuarioDAO = new UsuarioDAO(conexao))
                using (var logTentativaLogin = new LogTentativaLogin())
                using (var logTentativaLoginDAO = new LogTentativaLoginDAO(conexao))
                {
                    usuario.Login = login;

                    logTentativaLogin.Login = usuario.Login;

                    logTentativaLoginDAO.Cadastrar(logTentativaLogin, null);

                    if (!usuarioDAO.VerificarLoginESenha(usuario, senha, null))
                        return BadRequest(new
                        {
                            Sucesso = false,
                            Mensagem = "Login e/ou senha inválido(s)!"
                        });

                    var symmetricKey = Convert.FromBase64String(configuracao["SecurityKey"]);
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var subject = new ClaimsIdentity();
                    subject.AddClaim(new Claim("login", usuario.Login));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var stoken = tokenHandler.CreateToken(tokenDescriptor);

                    logTentativaLoginDAO.MarcarComoLoginEfetuado(logTentativaLogin, null);

                    return Ok(new
                    {
                        Sucesso = true,
                        Token = tokenHandler.WriteToken(stoken)
                    });
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