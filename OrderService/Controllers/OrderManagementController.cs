using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagementOrders")]
    public class OrderManagementController : ControllerBase
    {
        [HttpGet]
        public IActionResult EditOrders(Guid OrderId)
        {
            return Ok(true);
        }
    }
}
