using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{  
    List<Order> GetRunningOrders();
    List<OrderItem> GetOrderItemsByOrderId(int orderId);
    List<Order>GetFinishedOrders();
    void UpdateOrderStatus(Order order, OrderStatus newStatus);
    void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus newStatus);
    Order? GetOrderById(int id);
    int CreateOrder(int tableId);
    bool OrderItemExists(int orderId, int menuItemId, string comment);
    void IncreaseQuantity(int orderId, int menuItemId);
    void AddOrderItem(int orderId, int menuItemId, string comment);
    Order ?GetActiveOrderForTable(int tableId);
}



  
