using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
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

                int id = EventUtils.EXEC_SP_INSERTAREVENTO(connectionString, title, description, eventDate, commandersTimeSpan, onlyCommanders,
                    membersTimeSpan, userCreate, genenalBand, 1);

                JsonArray squads = json["squads"]!.AsArray();
                List<long> ids = squads.Select(n => long.Parse(n.ToString())).ToList();

                foreach(long i in  ids)
                {
                    _logger.LogInformation($"{i}");
                    EventUtils.EXEC_SP_INSERT_DETALLE_ASISTENCIA(connectionString, long.Parse(id.ToString()), i);
                }

                return Ok(new
                {
                    ok = true,
                    message = $"Evento creado exitosamente {id}"
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear evento {ex}" });
            }
        }

        [HttpGet]
        [Route("get_events")]
        public IActionResult GetEvents([FromQuery] long idEscuadra)
        {
            try
            {
                // Definir las fechas para el mes actual
                DateTime fechaInicio = new(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, fechaInicio, fechaFin, 1);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear evento {ex}" });
            }
        }

        [HttpDelete]
        [Route("delete_event")]
        public IActionResult DeleteEvent([FromQuery] long idEvent)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                
                //EventUtils.EXEC_SP_DELETE_DETALLE_EVENTO(connectionString, idEvent);
                EventUtils.EXEC_SP_DELETE_EVENTO(connectionString, idEvent);

                return Ok(new
                {
                    ok = true,
                    message = "Evento eliminado exitosamente."
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al eliminar evento {ex}" });
            }
        }

        [HttpGet]
        [Route("get_events_by_squad")]
        public IActionResult GetEventsBySquad([FromQuery] long idEscuadra)
        {
            try
            {
                // Definir las fechas para el mes actual
                DateTime fechaInicio = DateTime.Now.Date;
                DateTime fechaFin = DateTime.Now.Date;

                _logger.LogInformation($"fechaInicio == {fechaInicio}");
                _logger.LogInformation($"fechaFin == {fechaFin}");
                _logger.LogInformation($"idEscuadra == {idEscuadra}");

                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, fechaInicio, fechaFin, 1);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear evento {ex}" });
            }
        }

        [HttpGet]
        [Route("get_events_by_filters")]
        public IActionResult GetEventsByFilters([FromQuery] long idEscuadra, [FromQuery] DateTime date)
        {
            try
            {

                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<Event> list = EventUtils.EXEC_SP_GET_EVENTS_BY_ID_SQUAD(connectionString, idEscuadra, date.Date, date.Date, 1);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear evento {ex}" });
            }
        }

        [HttpGet]
        [Route("get_squad_by_event_id")]
        public IActionResult GetSquadByEventId([FromQuery] long id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<Escuadras> list = EventUtils.EXEC_SP_GET_ESCUADRAS_BY_EVENT(connectionString, id);
                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(200, new { message = $"Error base de datos {sqlEx}" });
            }
            catch (Exception ex)
            {
                return StatusCode(200, new { message = $"Error de servidor {ex}" });
            }
        }


    }
}
