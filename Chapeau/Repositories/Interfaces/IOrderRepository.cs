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
    public Order CreateOrderWithItems(Order order);
    void AddItemsToExistingOrder(int orderId, List<OrderItem> items);
    Order GetActiveOrderForTable(int tableId);
}



