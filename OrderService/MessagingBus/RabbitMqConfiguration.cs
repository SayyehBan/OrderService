namespace OrderService.MessagingBus;

public class RabbitMqConfiguration
{
    public string Hostname { get; set; }
    public string QueueName_BasketCheckout { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
