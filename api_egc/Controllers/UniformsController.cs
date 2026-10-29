using api_egc.Middles;
using api_egc.Models;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniformsController(ILogger<UniformsController> logger, IConfiguration configuration) : Controller
    {
        private readonly ILogger<UniformsController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... UniformsController"
            });
        }

        [HttpGet]
        [Route("get_uniforms")]
        public IActionResult GetUniforms([FromQuery] string? name, [FromQuery] int state = 0)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                string search = string.IsNullOrWhiteSpace(name) ? "%" : name.ToLower();

                List<Uniform> data = UniformsMiddles.EXEC_SP_GET_UNIFORM(connectionString, search, state);

                return Ok(new
                {
                    ok = true,
                    list = data
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
        [Route("insert_uniform")]
        public IActionResult InsertUniform([FromBody] Uniform uniform)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;

                UniformsMiddles.EXEC_SP_INSERT_UNIFORM(connectionString, uniform);

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

        [HttpPut]
        [Route("update_uniform")]
        public IActionResult UpdateUniform([FromBody] Uniform uniform)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;

                UniformsMiddles.EXEC_SP_UPDATE_UNIFORM(connectionString, uniform);

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
