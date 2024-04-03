using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using OrderService.Model.Services;
using SayyehBanTools.ConnectionDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); builder.Services.AddDbContext<OrderDataBaseContext>(o => o.UseSqlServer
                (SqlServerConnection.ConnectionString("pyxVN5Nd9YMp5Rw+Qm1CVw==", "G67SszvPH7yl16X1HITRrA==", "/sdZ5/rua1NX9Bq+MGFRUA==", "CZ4QU6A8k/il67ImzI7Rqg==", "e7p88q1ib2k7k9a8", "h54pm8tvzqr78mg4")));

builder.Services.AddTransient<IOrderService, ROrderService>();
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
