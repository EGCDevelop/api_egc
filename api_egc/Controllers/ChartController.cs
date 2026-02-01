using api_egc.Models;
using api_egc.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChartController(ILogger<ChartController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ChartController> _logger = logger;

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... ChartController"
            });
        }

        [HttpGet]
        [Route("get_data_from_attendance_charet")]
        public IActionResult GET_DATA_FROM_ATTENDANCE_CHART([FromQuery] long eventId, [FromQuery] long squadId)
        {
            try
            {
                //string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                string connectionString = _configuration.GetConnectionString(ConfigController.CurrentEnvironment)!;
                List<AttendanceChartDTO> list = ChartUtils.EXEC_SP_GET_DATA_FROM_ATTENDANCE_CHART(connectionString, eventId, squadId);

                return Ok(new
                {
                    ok = true,
                    list
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
