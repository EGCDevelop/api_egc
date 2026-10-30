using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(ILogger<OrderController> logger, IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<OrderController> _logger = logger;

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... OrderController"
            });
        }
    }
}
