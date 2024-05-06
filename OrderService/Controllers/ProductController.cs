using Microsoft.AspNetCore.Mvc;
using OrderService.Model.Services.ProductServices;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IVerifyProductService verifyProductService;
    private readonly IProductService productService;

    public ProductController(IVerifyProductService verifyProductService
        , IProductService productService)
    {
        this.verifyProductService = verifyProductService;
        this.productService = productService;
    }


    [HttpGet]
    public IActionResult Verify(Guid id)
    {
        var product = productService.GetProduct(new ProductDto { ProductId = id });
        return Ok(verifyProductService.Veryfy(new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
        }));
    }
}
