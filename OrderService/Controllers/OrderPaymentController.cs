﻿using Microsoft.AspNetCore.Mvc;
using OrderService.Model.Services;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPaymentController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderPaymentController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpPost]
        public IActionResult Post(Guid OrderId)
        {
            return Ok(orderService.RequestPayment(OrderId));
        }
    }
}
