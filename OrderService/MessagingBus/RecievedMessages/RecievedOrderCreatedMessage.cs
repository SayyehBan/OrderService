using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Model.Services.RegisterOrderServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SayyehBanTools.MessagingBus.RabbitMQ.Connection;
using SayyehBanTools.MessagingBus.RabbitMQ.Model;
using System.Text;

namespace OrderService.MessagingBus.RecievedMessages;

public class RecievedOrderCreatedMessage : BackgroundService
{
    private readonly RabbitMqConnectionSettings _rabbitMqConnectionSettings;
    private readonly RabbitMQConnection rabbitMQConnection;
    private readonly IRegisterOrderService registerOrderService;
    private readonly string _queueName;
    public RecievedOrderCreatedMessage(RabbitMQConnection rabbitMQConnection, IRegisterOrderService registerOrderService, IOptions<RabbitMqConnectionSettings> rabbitMqConnectionSettings)
    {
        _rabbitMqConnectionSettings = rabbitMqConnectionSettings.Value;
        _queueName = _rabbitMqConnectionSettings.queue;
        this.rabbitMQConnection = rabbitMQConnection;
        this.rabbitMQConnection.CreateRabbitMQConnection();
        this.rabbitMQConnection.Channel = rabbitMQConnection.Connection.CreateModel();
        this.rabbitMQConnection.Channel.QueueDeclare(queue: _queueName, durable: true,
            exclusive: false, autoDelete: false, arguments: null);
        this.registerOrderService = registerOrderService;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(rabbitMQConnection.Channel);

        consumer.Received += (sender, eventArg) =>
        {
            var body = Encoding.UTF8.GetString(eventArg.Body.ToArray());
            var basket = JsonConvert.DeserializeObject<BasketDto>(body);


            //ثبت سفارش
            var resultHandle = HandleMessage(basket);

            if (resultHandle)
                rabbitMQConnection.Channel.BasicAck(eventArg.DeliveryTag, false);
        };
        rabbitMQConnection.Channel.BasicConsume(queue: _queueName, false, consumer);


        return Task.CompletedTask;
    }
    private bool HandleMessage(BasketDto basket)
    {
        return registerOrderService.Execute(basket);
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
public class BasketItem
{
public string BasketItemId { get; set; }
public Guid ProductId { get; set; }
public string Name { get; set; }
public int Price { get; set; }
public int Quantity { get; set; }
}

public class BasketDto
{
public string BasketId { get; set; }
public string FirstName { get; set; }
public string LastName { get; set; }
public string PhoneNumber { get; set; }
public string Address { get; set; }
public string PostalCode { get; set; }
public string UserId { get; set; }
public int TotalPrice { get; set; }
public List<BasketItem> BasketItems { get; set; }
public string MessageId { get; set; }
public DateTime Creationtime { get; set; }
}