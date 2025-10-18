using System.Data.SqlClient;
using System.Text.Json.Nodes;
using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController(ILogger<MemberController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MemberController> _logger = logger;


        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... MemberController"
            });
        }

        [HttpGet]
        [Route("get_member_like")]
        public IActionResult GetMemberLike([FromQuery] string like, [FromQuery] long squadId, [FromQuery] long schoolId,
            [FromQuery] int isNew, [FromQuery] int memberState)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                int year = DateTime.Now.Year;

                string search = string.IsNullOrWhiteSpace(like) ? "%" : like.ToLower();

                List<MemberDTO> list = MemberUtils.EXEC_SP_GET_INTEGRANTE_LIKE(connectionString, year, search, squadId, schoolId, isNew, memberState);
                
                
                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(200, new { message = $"Error al hacer login {ex}" });
            }
        }

        [HttpGet]
        [Route("get_member_for_instructor")]
        public IActionResult GetMemberForInstructor([FromQuery] string? like, [FromQuery] long? squadId, [FromQuery] long? schoolId,
            [FromQuery] int? isNew, [FromQuery] int? memberState, [FromQuery] int? career)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                int year = DateTime.Now.Year;

                string search = string.IsNullOrWhiteSpace(like) ? "%" : like.ToLower();

                List<MemberDTO> list = MemberUtils.EXEC_SP_GET_INTEGRANTE_FOR_INSTRUCTOR(connectionString, year, search, squadId, schoolId, isNew, memberState, career);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(200, new { message = $"Error al hacer login {ex}" });
            }
        }

        [HttpPut]
        [Route("update_member")]
        public IActionResult UpdateMember([FromBody] JsonObject json)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                // DATOS DE INTEGRANTE
                long id = long.Parse(json["memberId"]!.ToString());
                string firstName = json["firstName"]!.ToString();
                string lastName = json["lastName"]!.ToString();
                string cellPhone = json["cellPhone"]!.ToString();
                long squadId = long.Parse(json["squadId"]!.ToString());
                long positionId = long.Parse(json["positionId"]!.ToString());
                long isActive = long.Parse(json["isActive"]!.ToString());
                long isAncient = long.Parse(json["isAncient"]!.ToString());

                // DATOS DE CARRERA
                long establecimientoId = long.Parse(json["establecimientoId"]!.ToString());
                string anotherEstablishment = json["anotherEstablishment"]!.ToString();
                long courseId = long.Parse(json["courseId"]!.ToString());
                string courseName = json["courseName"]!.ToString();
                long degreeId = long.Parse(json["degreeId"]!.ToString());
                string section = json["section"]!.ToString();

                // OTROS DATOS
                string fatherName = json["fatherName"]!.ToString();
                string fatherCell = json["fatherCell"]!.ToString();

                MemberUtils.EXEC_SP_UPDATE_MEMBER(connectionString, id, firstName, lastName, cellPhone, squadId, positionId, 
                    isActive, isAncient, establecimientoId, anotherEstablishment, courseId, courseName, degreeId, section,
                    fatherName, fatherCell);

                return Ok(new
                {
                    ok = true,
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
