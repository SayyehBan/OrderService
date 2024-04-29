using Newtonsoft.Json;
using OrderService.Model.Links;
using OrderService.Model.Services.ProductServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SayyehBanTools.MessagingBus.RabbitMQ.Connection;
using System.Text;

namespace OrderService.MessagingBus.RecievedMessages;

public class ReceivedUpdateProductNameMessage : BackgroundService
{
    private readonly RabbitMQConnection rabbitMQConnection; 
    private readonly IProductService productService;
    public ReceivedUpdateProductNameMessage(RabbitMQConnection rabbitMQConnection, IProductService productService)
    {
        this.rabbitMQConnection = rabbitMQConnection;
        this.rabbitMQConnection.CreateRabbitMQConnection();
        this.rabbitMQConnection.Channel = rabbitMQConnection.Connection.CreateModel();


        this.rabbitMQConnection.Channel.ExchangeDeclare(LinkRabbitMQ.UpdateProductName, ExchangeType.Fanout, true, false);
        this.rabbitMQConnection.Channel.QueueDeclare(queue: LinkRabbitMQ.Order_GetMessageOnUpdateProductName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        this.rabbitMQConnection.Channel.QueueBind(LinkRabbitMQ.Order_GetMessageOnUpdateProductName, LinkRabbitMQ.UpdateProductName, "");
        this.productService = productService;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(rabbitMQConnection.Channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var updateCustomerFullNameModel = JsonConvert.DeserializeObject<UpdateProductNameMessage>(content);

            var resultHandeleMessage = HandleMessage(updateCustomerFullNameModel);
            if (resultHandeleMessage)
                rabbitMQConnection.Channel.BasicAck(ea.DeliveryTag, false);
        };
        rabbitMQConnection.Channel.BasicConsume(LinkRabbitMQ.Order_GetMessageOnUpdateProductName, false, consumer);
        return Task.CompletedTask;
    }
    private bool HandleMessage(UpdateProductNameMessage updateProduct)
    {
        return productService.UpdateProductName(updateProduct.Id, updateProduct.NewName);
    }
}
public class UpdateProductNameMessage
{
public Guid Id { get; set; }
public string NewName { get; set; }
}