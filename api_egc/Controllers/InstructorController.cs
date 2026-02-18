using api_egc.Models.Instructors;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json.Nodes;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstructorController(ILogger<InstructorController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<InstructorController> _logger = logger;


        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... InstructorController"
            });
        }

        [HttpGet]
        [Route("get_assigned_squads_instructors")]
        public IActionResult GetAssignedSquadsInstructors([FromQuery] long idInstructor)
        {
            try
            {
                //string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<EscuadrasInstructoresDTO> data = InstructorUtils.EXEC_SP_GET_ASSIGNED_SQUADS_INSTRUCTORS(connectionString, idInstructor);

                return Ok(new
                {
                    data
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error {ex}" });
            }
        }

        [HttpPut]
        [Route("update_instructor_profile")]
        public IActionResult UpdateInstructorProfile([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;

                long id = long.Parse(json["id"]!.ToString());
                string name = json["name"]!.ToString();
                string lasrName = json["lastName"]!.ToString();
                string phone = json["phone"]!.ToString();
                string email = json["email"]!.ToString();

                InstructorUtils.EXEC_SP_UPDATE_INSTRUCTOR_PROFILE(connectionString, id, name, lasrName, phone, email);

                return Ok(new
                {
                    ok = true,
                    id,
                    nombre = name,
                    apellido = lasrName,
                    telefono = phone,
                    correo = email
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error {ex}" });
            }
        }

        [HttpGet]
        [Route("get_instructor")]
        public IActionResult GetInstructor([FromQuery] string? like, [FromQuery] int state, [FromQuery] int puesto,
            [FromQuery] int usuarioId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                string search = string.IsNullOrWhiteSpace(like) ? "%" : like.ToLower();

                _logger.LogInformation($"search = {search}");
                _logger.LogInformation($"state = {state}");
                _logger.LogInformation($"puesto = {puesto}");
                _logger.LogInformation($"usuarioId = {usuarioId}");


                List<Instructor> list = InstructorUtils.EXEC_SP_GET_INSTRUCTORS_BY_FILTERS(connectionString, state, puesto, search, usuarioId);

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

        [HttpPost]
        [Route("create_instructor")]
        public IActionResult CreateInstructor([FromBody] InstructorPayload json)
        {
            try
            {
                if (json == null) return BadRequest("El cuerpo de la petición está vacío");

                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                InstructorUtils.TRANSACTION_INSERT_INSTRUCTOR(connectionString, json);

                return Ok(new
                {
                    ok = true,
                });

            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error SQL CreateInstructor {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error CreateInstructor {ex}" });
            }
        }

        [HttpPut]
        [Route("update_instructor")]
        public IActionResult UpdateInstructor([FromBody] InstructorPayload json)
        {
            try
            {
                if (json == null) return BadRequest("El cuerpo de la petición está vacío");

                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;

                InstructorUtils.TRANSACTION_UPDATE_INSTRUCTOR(connectionString, json);

                return Ok(new
                {
                    ok = true,
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error SQL UpdateInstructor {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false, message = $"Error UpdateInstructor {ex}" });
            }
        }
    }
}
