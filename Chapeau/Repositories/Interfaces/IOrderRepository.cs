using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{
    List<Order> GetRunningOrders();
    List<OrderItem> GetOrderItemsByOrderId(Order order);
    List<Order> GetFinishedOrders();
    void UpdateOrderStatus(Order order, OrderStatus status);
    void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status);
    Order? GetOrderById(Order order);
    Order CreateOrder(int tableId);
    void IncreaseOrderItemQuantity(OrderItem newOrderItem, int quantity);
    void AddOrderItemToOrder(OrderItem newOrderItem);
    Order GetActiveOrderForTable(int tableId);
}



