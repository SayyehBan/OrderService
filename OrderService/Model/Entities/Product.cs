﻿namespace OrderService.Model.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; }
}
