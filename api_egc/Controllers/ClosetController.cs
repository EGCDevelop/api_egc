using api_egc.Middles;
using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClosetController(ILogger<ClosetController> logger, IConfiguration configuration) : Controller
    {
       private readonly ILogger<ClosetController> _logger = logger;
       private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... ClosetController"
            });
        }

        [HttpGet]
        [Route("get_closet")]
        public IActionResult GetCloset([FromQuery] long squadId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                long currentSquadId = 0;
                
                if(squadId == 1 || squadId == 12 || squadId == 14 || squadId == 16)
                {
                    currentSquadId = 1;
                } else if (squadId == 2 || squadId == 13 || squadId == 15)
                {
                    currentSquadId = 2;
                } else
                {
                    currentSquadId = 6;
                }
                
                List<Closet> list = ClosetMiddles.EXEC_SP_GET_CLOSET_BY_SQUAD(connectionString, currentSquadId);

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
        [Route("get_closet_dto")]
        public IActionResult GetClosetDto([FromQuery] long squadId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                long currentSquadId = 0;

                if (squadId == 1 || squadId == 12 || squadId == 14 || squadId == 16)
                {
                    currentSquadId = 1;
                }
                else if (squadId == 2 || squadId == 13 || squadId == 15)
                {
                    currentSquadId = 2;
                }
                else
                {
                    currentSquadId = 6;
                }

                List<ClosetDTO> list = ClosetMiddles.EXEC_SP_GET_CLOSET_BY_SQUAD_DTO(connectionString, currentSquadId);

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
        [Route("get_closet_summary")]
        public IActionResult GetClosetSummary()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                ClosetSummary gastadores = ClosetMiddles.EXEC_SP_GET_CLOSET_SUMMARY(connectionString, 1);
                ClosetSummary batonistas = ClosetMiddles.EXEC_SP_GET_CLOSET_SUMMARY(connectionString, 2);
                ClosetSummary banda = ClosetMiddles.EXEC_SP_GET_CLOSET_SUMMARY(connectionString, 6);
                List<ClosetSummary> list = [gastadores, batonistas, banda];

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
        [Route("get_closet_summary_by_squad")]
        public IActionResult GetClosetSummaryBySquad([FromBody] long squadId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                ClosetSummary data = ClosetMiddles.EXEC_SP_GET_CLOSET_SUMMARY(connectionString, squadId);
                

                return Ok(new
                {
                    ok = true,
                    data
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

        [HttpPost]
        [Route("update_closet")]
        public IActionResult UpdateCloset([FromBody] List<Closet> closets)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                ClosetMiddles.TransactionUniformsUpdated(connectionString, closets);

                return Ok(new {
                    ok = true 
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
