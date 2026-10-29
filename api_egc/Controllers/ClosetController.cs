using api_egc.Middles;
using api_egc.Models;
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
                List<Closet> list = ClosetMiddles.EXEC_SP_GET_CLOSET_BY_SQUAD(connectionString, squadId);

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

        [HttpPost]
        [Route("update_closet")]
        public IActionResult UpdateCloset([FromBody] List<Uniform> uniforms)
        {
            try
            {
                foreach (Uniform uniform in uniforms)
                {
                    _logger.LogInformation($"Uniform: {uniform.Id} | {uniform.Name} | {uniform.Price} | {uniform.State}");
                }

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
