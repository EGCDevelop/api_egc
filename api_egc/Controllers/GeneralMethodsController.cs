using System.Data.SqlClient;
using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneralMethodsController(ILogger<GeneralMethodsController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<GeneralMethodsController> _logger = logger;


        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... GeneralMethodsController"
            });
        }

        [HttpGet]
        [Route("get_squads")]
        public IActionResult GetSquads()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<Escuadras> list = GeneralMethodsUtils.EXEC_SP_GET_ESCUADRA(connectionString);

                return Ok(new
                {
                    ok = true,
                    list
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { ok = false,  message = $"Error SQL {ex}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ok = false,  message = $"Error {ex}" });
            }
        }

        [HttpPost]
        [Route("insert_member_per_year")]
        public IActionResult InsertMemberPerYear()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<IntegrantePerYearDto>  list = GeneralMethodsUtils.EXEC_SP_GET_BY_INSERT_PER_YEAR(connectionString);

                foreach(IntegrantePerYearDto dto in list)
                {
                    GeneralMethodsUtils.EXEC_SP_INSERT_MEMEBER_PER_YEAR(connectionString, dto.INTIdIntegrante,
                        dto.INTESCIdEscuadra, dto.INTPUIdPuesto);
                }

                return Ok(new
                {
                    ok = true,
                    message = "Integrantes por año insertados correctamente"
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
