using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Model.Services;
using SayyehBanTools.ConnectionDB;
using OrderService.Model.Services.ProductServices;
using OrderService.Model.Services.RegisterOrderServices;
using SayyehBanTools.ConfigureService;
using SayyehBanTools.MessagingBus.RabbitMQ.Model;
using OrderService.MessagingBus.RecievedMessages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); 
builder.Services.AddDbContext<OrderDataBaseContext>(o => o.UseSqlServer
                (SqlServerConnection.ConnectionString("pyxVN5Nd9YMp5Rw+Qm1CVw==", "G67SszvPH7yl16X1HITRrA==", "/sdZ5/rua1NX9Bq+MGFRUA==", "CZ4QU6A8k/il67ImzI7Rqg==", "e7p88q1ib2k7k9a8", "h54pm8tvzqr78mg4")), ServiceLifetime.Singleton);

builder.Services.AddTransient<IOrderService, ROrderService>();


var configureServices = new ConfigureServicesRabbitMQ();
configureServices.ConfigureService(builder.Services);
//RabbitMQ
builder.Services.Configure<RabbitMqConnectionSettings>(builder.Configuration
    .GetSection("RabbitMq"));

builder.Services.AddHostedService<RecievedOrderCreatedMessage>();
builder.Services.AddHostedService<RecievedPaymentOfOrderService>();
builder.Services.AddHostedService<ReceivedUpdateProductNameMessage>();
builder.Services.AddTransient<IProductService, RProductService>();
builder.Services.AddTransient<IRegisterOrderService, RRegisterOrderService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
