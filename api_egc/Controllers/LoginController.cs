using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController(ILogger<LoginController> logger, IConfiguration configuration) : Controller
    {

        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<LoginController> _logger = logger;


        [HttpGet]
        [Authorize]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new {
                ok = true,
                message = "GetPing ... LoginController"
            });
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] JsonObject json)
        {
            try
            {
                string username = json["username"]!.ToString();
                string password = json["password"]!.ToString();
                string version = json["version"]!.ToString();

                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;

                Versiones dbVersion = LoginUtils.EXEC_SP_VERSION_APP(connectionString);


                // comparamos si las versiones son iguales
                if (!version.Equals(dbVersion.VERNumero))
                {
                    return BadRequest(new
                    {
                        ok = false,
                        message = "¡ UPS ! debes actualizar a la versión más reciente"
                    });
                }


                // buscamos la informacion del integrante
                Member member = LoginUtils.EXEC_SP_GET_MEMBER_BY_USERNAME(connectionString, username);

                if (member == null)
                {
                    return Unauthorized(new
                    {
                        ok = false,
                        message = "Usuario o contraseña incorrectos"
                    });
                }


                // verificamos la contraseña
                bool isPasswordValid = PasswordHasher.VerifyPassword(password, member.INTPassword);

                if (!isPasswordValid)
                {
                    return Unauthorized(new
                    {
                        ok = false,
                        message = "Usuario o contraseña incorrectos"
                    });
                }

                // Generamos el token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, member.INTIdIntegrante.ToString()),
                        new Claim("FullName", $"{member.INTNombres} {member.INTApellidos}"),
                        new Claim("Role", member.INTPUIdPuesto.ToString())

                    }),
                    //Expires = DateTime.UtcNow.AddHours(1), // Token expira en 1 hora
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);


                return Ok(new
                {
                    ok = true,
                    member.INTIdIntegrante,
                    member.INTNombres,
                    member.INTApellidos,
                    member.INTESCIdEscuadra,
                    member.INTPUIdPuesto,
                    token = tokenHandler.WriteToken(token)
                });
            } 
            catch (Exception ex)
            {
                return StatusCode(200, new { message = $"Error al hacer login {ex}" });
            }
        }


        [HttpPut]
        [Route("change_password")]
        public IActionResult ChangePassword([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;

                string username = json["username"]!.ToString();
                string password = json["password"]!.ToString();

                LoginUtils.EXEC_SP_UPDATE_PASSWORD(connectionString, username, password);

                return Ok(new
                {
                    ok = true,
                    message = "contraseña actualizada correctamente"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(200, new { message = $"Error al hacer login {ex}" });
            }
        }


    }
}
