using api_egc.Models.Uniforms.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api_egc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController(ILogger<PaymentController> logger, IConfiguration configuration) : Controller
    {
        private readonly ILogger<PaymentController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        [Route("get_ping")]
        public IActionResult GetPing()
        {
            return Ok(new
            {
                ok = true,
                message = "GetPing ... PaymentController"
            });
        }

        [HttpPost]
        [Route("save_payment")]
        public IActionResult SavePayment([FromBody] SavePaymentHistory data)
        {
            try
            {
                _logger.LogInformation($"Payment = {data.Payment.ToString()}");
                _logger.LogInformation($"---------------------------------------------");
                _logger.LogInformation($"Details = {data.Details.ToString()}");
                return Ok(new
                {
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
