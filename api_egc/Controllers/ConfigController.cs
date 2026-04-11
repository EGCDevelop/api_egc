using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController(ILogger<ConfigController> logger) : Controller
    {
        private readonly ILogger<ConfigController> _logger = logger;
        public static string CurrentEnvironment = "DbEgcConnection";
        //public static string CurrentEnvironment = "DbEgcConnectionTest";

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... ConfigController"
            });
        }

        [HttpPost]
        [Route("change_enviroment")]
        public IActionResult ChangeEnviroment([FromBody] JsonObject json)
        {
            try
            {
                int state = int.Parse(json["state"]!.ToString());

                CurrentEnvironment = state == 1 ? "DbEgcConnection" : "DbEgcConnectionTest";
                //CurrentEnvironment = state == 1 ? "DbEgcConnectionTest" : "DbEgcConnectionTest";

                return Ok(new
                {
                    ok = true,
                    message = "Conexión cambiada exitosamente"
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
        [Route("get_enviroment")]
        public IActionResult GetEnviroment()
        {
            try
            {
                int state = 2;
                if (CurrentEnvironment.Equals("DbEgcConnection"))
                {
                    state = 1;
                } else
                {
                    state = 0;
                }

                return Ok(new
                {
                    ok = true,
                    state
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
