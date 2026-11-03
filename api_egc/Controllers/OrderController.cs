using api_egc.Middles;
using api_egc.Models.Uniforms;
using api_egc.Models.Uniforms.DTO;
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

        [HttpGet]
        [Route("get_order_by_filters")]
        public IActionResult GetOrderByFilters([FromHeader] long memeberId, [FromHeader] int financialState, [FromHeader] int deliveryState)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                List<UniformOrder> data = OrderMiddles.EXEC_SP_GET_ORDER_BY_FILTERS(connectionString, memeberId, financialState, deliveryState);

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
        [Route("save_order")]
        public IActionResult SaveOrder([FromBody] SaveOrdenDTO data)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DbEgcConnection")!;
                OrderMiddles.TRANSACTION_SAVE_ORDER(connectionString, data);

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
