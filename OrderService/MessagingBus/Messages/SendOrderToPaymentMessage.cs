using SayyehBanTools.MessagingBus.RabbitMQ.Model;

namespace OrderService.MessagingBus.Messages;

public class SendOrderToPaymentMessage :BaseMessage
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
}
