using OrderService.Model.Services.ProductServices;

namespace OrderContractTest;
public class OrderProductVerifyTest : IClassFixture<ConsumerPactClassFixture>
{
    private IMockProviderService _mockProviderService;
    private string _mockProviderServiceBaseUri;

    public OrderProductVerifyTest(ConsumerPactClassFixture fixture)
    {
        _mockProviderService = fixture.MockProviderService;
        _mockProviderService.ClearInteractions();
        _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
    }

    [Fact]
    public void Check_Product_Verify_Api()
    {
        //Arrange
        _mockProviderService.Given("There is correct data")
            .UponReceiving("Product information must be returned")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/api/product/verify/a6eba892-cd99-4f79-b2c8-76fbc6e18359",
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, object>
                 {
                           { "Content-Type", "application/json; charset=utf-8" }
                 },
                Body = Match.Type(new
                {
                    id = "a6eba892-cd99-4f79-b2c8-76fbc6e18359",
                    name = "some name"
                })
            });
        //Act
        IVerifyProductService verifyProduct = new VerifyProductService(
            new RestSharp.RestClient(_mockProviderServiceBaseUri));

        var result = verifyProduct.Veryfy(new ProductDto
        {
            ProductId = Guid.Parse("a6eba892-cd99-4f79-b2c8-76fbc6e18359")
        });
        //Assert

        Assert.NotNull(result);
        Assert.NotNull(result.Name);
    }
}