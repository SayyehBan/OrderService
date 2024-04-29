namespace OrderService.Model.Links;

public static class LinkRabbitMQ
{
    public static string OrderSendToPayment = "OrderSendToPayment";
    public static string PaymentDone = "PaymentDone";
    public static string BasketCheckout = "BasketCheckout";
    public static string Order_GetMessageOnUpdateProductName = "Order_GetMessageOnUpdateProductName";
    public static string UpdateProductName = "UpdateProductName";
}
