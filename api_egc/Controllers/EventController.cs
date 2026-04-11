using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json.Nodes;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController(ILogger<EventController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<EventController> _logger = logger;


        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... EventController"
            });
        }


        [HttpPost]
        [Route("create_event")]
        public IActionResult CreateEvent([FromBody] JsonObject json)
        {
            try
            {
                //string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;

                string title = json["title"]!.ToString();
                string description = json["description"]!.ToString();
                string userCreate = json["userCreate"]!.ToString();

                // Extraer fecha del evento
                DateTime eventDate;
                if (!DateTime.TryParse(json["eventDate"]?.ToString(), out eventDate))
                {
                    return BadRequest(new { message = "Formato de fecha inválido eventDate" });
                }

                // Extraer hora de entrada
                TimeOnly commandersEntry;
                if (!TimeOnly.TryParse(json["commandersEntry"]?.ToString(), out commandersEntry))
                {
                    return BadRequest(new { message = "Formato de hora inválido. Usa HH:mm:ss" });
                }

                TimeOnly membersEntry;
                if (!TimeOnly.TryParse(json["membersEntry"]?.ToString(), out membersEntry))
                {
                    return BadRequest(new { message = "Formato de hora inválido. Usa HH:mm:ss" });
                }

                TimeSpan commandersTimeSpan = commandersEntry.ToTimeSpan();
                TimeSpan membersTimeSpan = membersEntry.ToTimeSpan();

                bool onlyCommanders = json["onlyCommanders"]?.ToString() == "1";
                bool genenalBand = json["generalBand"]?.ToString() == "1";
                int eventType = int.Parse(json["eventType"]!.ToString());

                int id = EventUtils.EXEC_SP_INSERTAREVENTO(connectionString, title, description, eventDate, commandersTimeSpan, onlyCommanders,
                    membersTimeSpan, userCreate, genenalBand, 1, eventType);

                JsonArray squads = json["squads"]!.AsArray();
                List<long> ids = squads.Select(n => long.Parse(n.ToString())).ToList();

                foreach(long i in  ids)
                {
                    EventUtils.EXEC_SP_INSERT_DETALLE_ASISTENCIA(connectionString, long.Parse(id.ToString()), i);
                }

                return Ok(new
                {
                    ok = true,
                    message = $"Evento creado exitosamente {id}"
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
        [Route("get_events")]
        public IActionResult GetEvents([FromQuery] long idEscuadra, [FromQuery] int activo)
        {
            try
            {
                DateTime fechaInicio = new(DateTime.Now.Year, 1, 1);
                DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, fechaInicio, 
                    fechaFin, 1, activo);

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

        [HttpDelete]
        [Route("delete_event")]
        public IActionResult DeleteEvent([FromQuery] long idEvent)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;

                //EventUtils.EXEC_SP_DELETE_DETALLE_EVENTO(connectionString, idEvent);
                EventUtils.EXEC_SP_DELETE_EVENTO(connectionString, idEvent);

                return Ok(new
                {
                    ok = true,
                    message = "Evento eliminado exitosamente."
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
        [Route("get_events_by_squad")]
        public IActionResult GetEventsBySquad([FromQuery] long idEscuadra, [FromQuery] int activo)
        {
            try
            {
                // Definir las fechas para el mes actual
                DateTime fechaInicio = Utils.Utils.getCurrentDateGMT6();
                DateTime fechaFin = Utils.Utils.getCurrentDateGMT6();

                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, fechaInicio.Date, 
                    fechaFin.Date, 1, activo);

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
        [Route("get_events_by_filters")]
        public IActionResult GetEventsByFilters([FromQuery] long idEscuadra, [FromQuery] DateTime date,
            [FromQuery] int activo)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, date.Date,
                    date.Date, 1, activo);

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
        [Route("get_squad_by_event_id")]
        public IActionResult GetSquadByEventId([FromQuery] long id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<Escuadras> list = EventUtils.EXEC_SP_GET_ESCUADRAS_BY_EVENT(connectionString, id);
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

        [HttpPut]
        [Route("end_event")]
        public IActionResult EndEvent([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                int eventId = int.Parse(json["eventId"]!.ToString());
                string username = json["username"]!.ToString();
                DateTime endDate = Utils.Utils.getCurrentDateGMT6();
                string commentExit = "Evento finalizado";

                EventUtils.TRANSACTION_END_EVENT(connectionString, eventId, username, endDate, commentExit);

                return Ok(new
                {
                    ok = true,
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
