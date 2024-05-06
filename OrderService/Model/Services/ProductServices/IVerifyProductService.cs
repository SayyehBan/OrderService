using Newtonsoft.Json;
using RestSharp;

namespace OrderService.Model.Services.ProductServices;

public interface IVerifyProductService
{
    VerifyProductDto Veryfy(ProductDto product);
}

public class RVerifyProductService : IVerifyProductService
{
    private readonly RestClient restClient;

    public RVerifyProductService(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public VerifyProductDto Veryfy(ProductDto product)
    {
        var request = new RestRequest($"/api/product/verify/{product.ProductId}", Method.Get);
        var response = restClient.Execute(request);
        var productOnRemote = JsonConvert.DeserializeObject<ProductVeryfyOnServerProductDto>(response.Content);
        return Verify(product, productOnRemote);

    }

    private VerifyProductDto Verify(ProductDto local, ProductVeryfyOnServerProductDto remote)
    {
        if (local.Name == remote.ProductName)
        {
            return new VerifyProductDto
            {
                IsCorrect = true,
            };
        }
        else
        {
            return new VerifyProductDto
            {
                Name = remote.ProductName,
                IsCorrect = false,
            };
        }
    }
}
public class VerifyProductDto
{
    public string Name { get; set; }
    public bool IsCorrect { get; set; }
}

public class ProductVeryfyOnServerProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
}