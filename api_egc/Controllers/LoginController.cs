using api_egc.Models;
using api_egc.Models.Instructors;
using api_egc.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

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

                if(member != null)
                {
                    // Verificamos la contraseña
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
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = _configuration["Jwt:Issuer"],
                        Audience = _configuration["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature
                        )
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    // Guardamos token y bitácora
                    LoginUtils.EXEC_SP_UPDATE_TOKEN(connectionString, username, tokenHandler.WriteToken(token));
                    LoginUtils.EXEC_SP_INSERT_BITACORA(connectionString, member.INTIdIntegrante);

                    return Ok(new
                    {
                        ok = true,
                        member.INTIdIntegrante,
                        member.INTNombres,
                        member.INTApellidos,
                        member.INTESCIdEscuadra,
                        member.INTPUIdPuesto,
                        token = tokenHandler.WriteToken(token),
                        username
                    });
                }
                // buscamos informacion del instructor
                else
                {
                    InstructorDTO instructor = LoginUtils.EXEC_SP_GET_INSTRUCTOR_BY_USERNAME(connectionString, username);

                    if(instructor != null)
                    {
                        // Verificamos la contraseña del instructor
                        bool isPasswordValid = PasswordHasher.VerifyPassword(password, instructor.INSPassword);

                        if (!isPasswordValid)
                        {
                            return Unauthorized(new
                            {
                                ok = false,
                                message = "Usuario o contraseña incorrectos"
                            });
                        }

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.Name, instructor.INSId.ToString()),
                                new Claim("FullName", $"{instructor.INSNombre} {instructor.INSApellido}"),
                                new Claim("Role", "Instructor") // o instructor.INS_Role si lo tienes
                            }),
                            Expires = DateTime.UtcNow.AddHours(1),
                            Issuer = _configuration["Jwt:Issuer"],
                            Audience = _configuration["Jwt:Audience"],
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature
                            )
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        LoginUtils.EXEC_SP_UPDATE_TOKEN_INSTRUCTOR(connectionString, username, tokenHandler.WriteToken(token));
                        LoginUtils.EXEC_SP_INSERT_BITACORA_INSTRUCTOR(connectionString, instructor.INSId);
                        List<EscuadrasInstructoresDTO> squadList = InstructorUtils.EXEC_SP_GET_ASSIGNED_SQUADS_INSTRUCTORS(connectionString, instructor.INSId);
                        List<int> squadIdList = [];
                        foreach (EscuadrasInstructoresDTO squad in squadList)
                        {
                            squadIdList.Add(squad.IdEscuadra);
                        }


                        return Ok(new
                        {
                            ok = true,
                            id = instructor.INSId,
                            nombre = instructor.INSNombre,
                            apellido = instructor.INSApellido,
                            telefono = instructor.INSTelefono,
                            correo = instructor.INSCorreo,
                            idPuesto = instructor.INTPIId,
                            area = instructor.INSArea,
                            token = tokenHandler.WriteToken(token),
                            rol = instructor.INSRol,
                            squadIdList,
                            username
                        });
                    }
                }

                return Unauthorized(new
                {
                    ok = false,
                    message = "Usuario o contraseña incorrectos"
                });

            }
            catch(SqlException sqlEx)
            {
                return StatusCode(500, new { message = $"Error en sql = {sqlEx}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al hacer login {ex}" });
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
                return StatusCode(500, new { message = $"Error al hacer login {ex}" });
            }
        }

        [HttpPut]
        [Route("change_password_instructor")]
        public IActionResult ChangePasswordInstructor([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;

                string username = json["username"]!.ToString();
                string password = json["password"]!.ToString();

                LoginUtils.SP_UPDATE_PASSWORD_INSTRUCTOR(connectionString, username, password);

                return Ok(new
                {
                    ok = true
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al hacer login {ex}" });
            }
        }


    }
}
