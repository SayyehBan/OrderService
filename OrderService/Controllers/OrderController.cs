using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.Services;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(policy: "GetOrders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;
    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [HttpGet]
    public IActionResult Get()
    {

        string UserId;

        UserId = User.Claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

        Console.WriteLine($"Get Order For UserId: {UserId}");
        var orders = orderService.GetOrdersForUser(UserId);
        return Ok(orders);
    }

    [HttpGet("{OrderId}")]
    public IActionResult Get(Guid OrderId)
    {
        var order = orderService.GetOrderById(OrderId);
        return Ok(order);
    }
}
