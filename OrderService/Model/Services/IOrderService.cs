using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Model.Entities;

namespace OrderService.Model.Services;
public interface IOrderService
{

    List<OrderDto> GetOrdersForUser(string UserId);
    OrderDetailDto GetOrderById(Guid Id);
}


public class ROrderService : IOrderService
{
    private readonly OrderDataBaseContext context;

    public ROrderService(OrderDataBaseContext context)
    {
        this.context = context;
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
        }).ToList();
        return orders;
    }
}



public class OrderDto
{
    public Guid Id { get; set; }
    public int ItemCount { get; set; }
    public int TotalPrice { get; set; }
    public bool OrderPaid { get; set; }
    public DateTime OrderPlaced { get; set; }

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
    public List<OrderLineDto> OrderLines { get; set; }

}

public class OrderLineDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}