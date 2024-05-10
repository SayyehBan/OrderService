﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.Services;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
        string UserId = "1";
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
