using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;

namespace OrderContractTest;

public class ConsumerPactClassFixture : IDisposable
{
    private readonly IHost server;
    public Uri ServerUri { get; }

    public ConsumerPactClassFixture()
    {
        ServerUri = new Uri("https://localhost:44304");
        server = Host.CreateDefaultBuilder()
                     .ConfigureWebHostDefaults(webBuilder =>
                     {
                         webBuilder.UseUrls(ServerUri.ToString());
                     })
                     .Build();
        server.Start();
    }

    public void Dispose()
    {
        server.Dispose();
    }
}