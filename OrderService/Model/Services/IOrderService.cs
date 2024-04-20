using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Infrastructure.Context;
using OrderService.MessagingBus;
using OrderService.MessagingBus.Messages;
using OrderService.Model.Dto;
using OrderService.Model.Entities;
using SayyehBanTools.MessagingBus.RabbitMQ.Model;
using SayyehBanTools.MessagingBus.RabbitMQ.SendMessage;

namespace OrderService.Model.Services;
public interface IOrderService
{

    List<OrderDto> GetOrdersForUser(string UserId);
    OrderDetailDto GetOrderById(Guid Id); 
    ResultDto RequestPayment(Guid OrderId);
}


public class ROrderService : IOrderService
{
    private readonly OrderDataBaseContext context;
    private readonly ISendMessages sendMessages;
    private readonly RabbitMqConnectionSettings _rabbitMqConnectionSettings;
    private readonly string _queueName;
    public ROrderService(OrderDataBaseContext context,ISendMessages sendMessages, IOptions<RabbitMqConnectionSettings> rabbitMqConnectionSettings)
    {
        _rabbitMqConnectionSettings = rabbitMqConnectionSettings.Value;
        this.context = context;
        this.sendMessages = sendMessages;
        _queueName = _rabbitMqConnectionSettings.queue;
    }

    public OrderDetailDto GetOrderById(Guid Id)
    {
        var orders = context.Orders.
               Include(p => p.OrderLines)
              .ThenInclude(p => p.Product)
             .FirstOrDefault(p => p.Id == Id);

        if (orders == null)
            throw new Exception("Order Not Found");
        var result = new OrderDetailDto
        {
            Id = orders.Id,
            Address = orders.Address,
            FirstName = orders.FirstName,
            LastName = orders.LastName,
            PhoneNumber = orders.PhoneNumber,
            UserId = orders.UserId,
            OrderPaid = orders.OrderPaid,
            OrderPlaced = orders.OrderPlaced,
            TotalPrice = orders.TotalPrice,
            PaymentStatus = orders.PaymentStatus,
            OrderLines = orders.OrderLines.Select(ol => new OrderLineDto
            {
                Id = ol.Id,
                Name = ol.Product.Name,
                Price = ol.Product.Price,
                Quantity = ol.Quantity,

            }).ToList(),

        };
        return result;

    }

    public List<OrderDto> GetOrdersForUser(string UserId)
    {
        var orders = context.Orders.
         Include(p => p.OrderLines)
        .Where(p => p.UserId == UserId)
        .Select(p => new OrderDto
        {
            Id = p.Id,
            OrderPaid = p.OrderPaid,
            OrderPlaced = p.OrderPlaced,
            ItemCount = p.OrderLines.Count(),
            TotalPrice = p.TotalPrice,
            PaymentStatus = p.PaymentStatus
        }).ToList();
        return orders;
    }

    public ResultDto RequestPayment(Guid OrderId)
    {
        var order = context.Orders.SingleOrDefault(p => p.Id == OrderId);
        if (order == null)
        {
            return new ResultDto
            {
                IsSuccess = false,
                Message = "سفارش پیدا نشد"
            };
        }
        // ارسال پیام پرداخت برای سرویس پرداخت
        SendOrderToPaymentMessage paymentMessage = new SendOrderToPaymentMessage()
        {
            Amount = order.TotalPrice,
            Creationtime = DateTime.UtcNow,
            MessageId = Guid.NewGuid(),
            OrderId = order.Id,
        };
        sendMessages.SendMessage(paymentMessage, _queueName);
        //تغییر وضعیت پرداخت سفارش
        order.RequestPayment();
        context.SaveChanges();
        return new ResultDto
        {
            IsSuccess = true,
            Message = "درخواست پرداخت ثبت شد"
        };
    }
}



public class OrderDto
{
    public Guid Id { get; set; }
    public int ItemCount { get; set; }
    public int TotalPrice { get; set; }
    public bool OrderPaid { get; set; }
    public DateTime OrderPlaced { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}


public class OrderDetailDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderPlaced { get; set; }
    public bool OrderPaid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public int TotalPrice { get; set; }
    public List<OrderLineDto> OrderLines { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public class OrderLineDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}