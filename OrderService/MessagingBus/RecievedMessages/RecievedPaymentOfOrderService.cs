using Newtonsoft.Json;
using OrderService.Infrastructure.Context;
using OrderService.Model.Links;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SayyehBanTools.MessagingBus.RabbitMQ.Connection;
using System.Text;

namespace OrderService.MessagingBus.RecievedMessages;

public class RecievedPaymentOfOrderService : BackgroundService
{
    private readonly RabbitMQConnection rabbitMQConnection;
    private readonly OrderDataBaseContext context;
    public RecievedPaymentOfOrderService(RabbitMQConnection rabbitMQConnection, OrderDataBaseContext context)
    {
        this.rabbitMQConnection = rabbitMQConnection;
        this.rabbitMQConnection.CreateRabbitMQConnection();
        this.rabbitMQConnection.Channel = rabbitMQConnection.Connection.CreateModel();
        this.rabbitMQConnection.Channel.QueueDeclare(queue: LinkRabbitMQ.PaymentDone, durable: true,
            exclusive: false, autoDelete: false, arguments: null);
        this.context = context;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(rabbitMQConnection.Channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var paymentDone = JsonConvert.
            DeserializeObject<PaymentOrderMessage>(content);
            var resultHandeleMessage = HandleMessage(paymentDone);
            if (resultHandeleMessage)
                rabbitMQConnection.Channel.BasicAck(ea.DeliveryTag, false);

        };

        rabbitMQConnection.Channel.BasicConsume(LinkRabbitMQ.PaymentDone, false, consumer);
        return Task.CompletedTask;

    }
    private bool HandleMessage(PaymentOrderMessage paymentOrderMessage)
    {
        var order = context.Orders.SingleOrDefault(p => p.Id == paymentOrderMessage.OrderId);
        order.PaymentIsDone();
        context.SaveChanges();
        return true;
    }
}
public class PaymentOrderMessage
{
    public Guid OrderId { get; set; }
}