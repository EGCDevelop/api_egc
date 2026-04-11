using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json.Nodes;

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
                
                if (asistencia)
                {
                    return Ok(new
                    {
                        ok = true,
                        message = "Integrante ya cuenta con asistencia"
                    });
                }
                
                if (!generales.Contains(puestoComandante))
                {
                    // Si no es un general procedemos a validar que el comandante y el integrante sean de la misma escuadra
                    IntegranteDto integrante = AsistenciaUtils.EXEC_SP_GET_INTEGRANTE_BY_ID(connectionString, idIntegrante);
                    long escuadraIntegrante = integrante.INTESCIdEscuadra;
                    bool esAutorizado = false;

                    if (escuadraComandante == 2 || escuadraComandante == 13)
                    {
                        HashSet<long> permitidos = [2, 13, 15];
                        if (permitidos.Contains(escuadraIntegrante)) esAutorizado = true;
                    }

                    else if (escuadraComandante == 1 || escuadraComandante == 12)
                    {
                        HashSet<long> permitidos = [1, 12, 14];
                        if (permitidos.Contains(escuadraIntegrante)) esAutorizado = true;
                    }

                    else if (escuadraComandante == escuadraIntegrante)
                    {
                        esAutorizado = true;
                    }

                    if (!esAutorizado)
                    {
                        return BadRequest(new
                        {
                            ok = false,
                            message = "El integrante no pertenece a una escuadra bajo su mando."
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
                var fullMessage = ex.InnerException != null
                                  ? $"{ex.Message} | Original: {ex.InnerException.Message}"
                                  : ex.Message;

                _logger.LogInformation($"fullMessage == {fullMessage}");
                _logger.LogInformation($"StackTrace == {ex.StackTrace}");

                return StatusCode(500, fullMessage);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("get_asistencia")]
        public IActionResult GetAsistencia([FromQuery] long idEscuadra, [FromQuery] DateTime date, [FromQuery] long eventId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<AsistenciaDto> list = [];

                bool exist = AsistenciaUtils.EXEC_SP_VALIDATE_EVENT_EXIST(connectionString, idEscuadra, eventId);

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
                var fullMessage = ex.InnerException != null
                                  ? $"{ex.Message} | Original: {ex.InnerException.Message}"
                                  : ex.Message;

                _logger.LogInformation($"fullMessage == {fullMessage}");
                _logger.LogInformation($"StackTrace == {ex.StackTrace}");

                return StatusCode(500, fullMessage);
            }
        }

        [HttpGet]
        //[Authorize]
        [Route("get_matriz_asistencia")]
        public IActionResult GetMatrizAsistencia([FromQuery] long idEscuadra = 0, [FromQuery] int tipoIntegrante = 2, 
            [FromQuery] int filtroPuesto = 0, [FromQuery] DateTime? fechaInicio = null)
        {
            try
            {
                _logger.LogInformation("GetMatrizAsistencia...");

                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                DateTime fechaBusqueda = fechaInicio ?? new DateTime(2026, 1, 1);

                _logger.LogInformation("uno");
                _logger.LogInformation($"idEscuadra = {idEscuadra}");
                _logger.LogInformation($"tipoIntegrante = {tipoIntegrante}");
                _logger.LogInformation($"filtroPuesto = {filtroPuesto}");
                _logger.LogInformation($"fechaBusqueda = {fechaBusqueda}");


                var list = AsistenciaUtils.EXEC_SP_REPORTE_ASISTENCIA_MATRIZ(connectionString, idEscuadra, 
                    tipoIntegrante, filtroPuesto, fechaBusqueda);
                _logger.LogInformation("dos");

                return Ok(new { ok = true, list });
            }
            catch (Exception ex)
            {
                var fullMessage = ex.InnerException != null
                                  ? $"{ex.Message} | Original: {ex.InnerException.Message}"
                                  : ex.Message;

                _logger.LogInformation($"fullMessage == {fullMessage}");
                _logger.LogInformation($"StackTrace == {ex.StackTrace}");

                return StatusCode(500, fullMessage);
            }
        }

        [HttpPut]
        [Route("register_extraordinary_departure")]
        public IActionResult RegisterExtraordinaryDeparture([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                string exitComment = json["exitComment"]!.ToString();
                long memberId = long.Parse(json["memberId"]!.ToString());
                long eventId = long.Parse(json["eventId"]!.ToString());
                DateTime exitDate = Utils.Utils.getCurrentDateGMT6();
                string username = json["username"]!.ToString();

                AsistenciaUtils.EXEC_SP_UPDATE_REGISTER_EXTRAORDINARY_DEPARTURE(connectionString, exitComment, memberId,
                    eventId, exitDate, username);

                return Ok(new
                {
                    ok = true,
                    message = $"Salida registrada exitosamente"
                });

            }
            catch (Exception ex)
            {
                var fullMessage = ex.InnerException != null
                                  ? $"{ex.Message} | Original: {ex.InnerException.Message}"
                                  : ex.Message;

                _logger.LogInformation($"fullMessage == {fullMessage}");
                _logger.LogInformation($"StackTrace == {ex.StackTrace}");

                return StatusCode(500, fullMessage);
            }
        }

        [HttpPut]
        [Route("register_justification_absence")]
        public IActionResult RegisterJustificationAbsence([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                string justificationComment = json["justificationComment"]!.ToString();
                long memberId = long.Parse(json["memberId"]!.ToString());
                long eventId = long.Parse(json["eventId"]!.ToString());
                long usernameId = long.Parse(json["usernameId"]!.ToString());
                string username = json["username"]!.ToString();

                _logger.LogInformation($"justificationComment == {justificationComment}");
                _logger.LogInformation($"memberId == {memberId}");
                _logger.LogInformation($"eventId == {eventId}");
                _logger.LogInformation($"usernameId == {usernameId}");
                _logger.LogInformation($"username == {username}");

                AsistenciaUtils.EXEC_SP_UPDATE_REGISTER_JUSTIFICATION_ABSENCE(connectionString, justificationComment, memberId,
                    eventId, usernameId, username);

                return Ok(new
                {
                    ok = true,
                    message = $"¡Justificación registrada exitosamente!"
                });

            }
            catch (Exception ex)
            {
                var fullMessage = ex.InnerException != null
                                  ? $"{ex.Message} | Original: {ex.InnerException.Message}"
                                  : ex.Message;

                _logger.LogInformation($"fullMessage == {fullMessage}");
                _logger.LogInformation($"StackTrace == {ex.StackTrace}");

                return StatusCode(500, fullMessage);
            }
        }
    }
}
