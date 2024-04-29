using OrderService.Infrastructure.Context;
using OrderService.Model.Entities;

namespace OrderService.Model.Services.ProductServices;

public interface IProductService
{
    Product GetProduct(ProductDto productDto);
    bool UpdateProductName(Guid ProductId, string productName);
}

public class RProductService : IProductService
{
    private readonly OrderDataBaseContext context;

    public RProductService(OrderDataBaseContext context)
    {
        this.context = context;
    }
    public Product GetProduct(ProductDto productDto)
    {
        var existProduct = context.Products.SingleOrDefault(p => p.ProductId == productDto.ProductId);
        if (existProduct != null)
            return existProduct;
        else
            return CreateNewProduct(productDto);
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
        Product newProduct = new Product()
        {
            ProductId = productDto.ProductId,
            Name = productDto.Name,
            Price = productDto.Price,
        };
        context.Products.Add(newProduct);
        context.SaveChanges();
        return newProduct;
    }

    public bool UpdateProductName(Guid ProductId, string productName)
    {
        var product = context.Products.Find(ProductId);
        if (product is not null)
        {
            product.Name = productName;
            context.SaveChanges();
        }
        return true;
    }

}


public class ProductDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}