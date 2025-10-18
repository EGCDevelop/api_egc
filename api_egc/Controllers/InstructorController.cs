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
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
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
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
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
    }
}
