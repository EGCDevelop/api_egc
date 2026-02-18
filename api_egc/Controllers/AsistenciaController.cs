using System.Text.Json.Nodes;
using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AsistenciaController(ILogger<AsistenciaController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<AsistenciaController> _logger = logger;


        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... LoginController"
            });
        }


        [HttpPost]
        [Authorize]
        [Route("register_attendance")]
        public IActionResult RegisterAttendance([FromBody] JsonObject json)
        {
            try
            {
                //string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;

                long idIntegrante = long.Parse(json["id"]!.ToString());
                long escuadraComandante = long.Parse(json["escuadra"]!.ToString());
                long puestoComandante = long.Parse(json["puesto"]!.ToString());
                long eventId = long.Parse(json["eventId"]!.ToString());
                long idRegistro = long.Parse(json["idRegistro"]!.ToString());
                HashSet<long> generales = [1, 2, 3, 4];


                bool asistencia = AsistenciaUtils.EXEC_SP_GET_ASISTENCIA_BY_INTEGRANTE(connectionString, idIntegrante, eventId);

                _logger.LogInformation($"asistencia == {asistencia}");

                if(asistencia)
                {
                    return Ok(new
                    {
                        ok = true,
                        message = "Asistencia registrada exitosamente"
                    });
                }

                if (!generales.Contains(puestoComandante))
                {
                    // Si no es un general procedemos a validar que el comandante y el integrante sean de la misma escuadra
                    IntegranteDto integrante = AsistenciaUtils.EXEC_SP_GET_INTEGRANTE_BY_ID(connectionString, idIntegrante);

                    if(escuadraComandante != integrante.INTESCIdEscuadra)
                    {
                        return BadRequest(new
                        {
                            ok = false,
                            message = "El integrante no es de la misma escuadra"
                        });
                    }
                }

                AsistenciaUtils.EXEC_SP_INSERT_ASISTENCIA(connectionString, idIntegrante, eventId, idRegistro);

                return Ok(new
                {
                    ok = true,
                    message = "Asistencia registrada exitosamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al hacer login {ex}" });
            }
        }


        [HttpGet]
        [Route("get_asistencia")]
        public IActionResult GetAsistencia([FromQuery] long idEscuadra, [FromQuery] DateTime date, [FromQuery] long eventId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<AsistenciaDto> list = [];

                bool exist = AsistenciaUtils.EXEC_SP_VALIDATE_EVENT_EXIST(connectionString, idEscuadra, eventId);

                _logger.LogInformation($"exist == {exist}");

                if (!exist)
                {
                    return Ok(new
                    {
                        ok = true,
                        list
                    });
                }

                list = AsistenciaUtils.EXEC_SP_GET_ASISTENCIA(connectionString, idEscuadra, date, eventId);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al hacer login {ex}" });
            }
        }


    }
}
