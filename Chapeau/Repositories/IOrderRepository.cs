using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{  
    List<Order> GetAllOrders();
    List<OrderItem> GetOrderItemsByOrderId(int orderId);
    List<Order>GetFinishedOrders();
    void UpdateOrderStatus(int orderId, OrderStatus newStatus);
    void UpdateOrderItemStatus(int orderItemId, OrderStatus status);
    Order? GetOrderById(int id);
    int CreateOrder(int tableId);
    bool OrderItemExists(int orderId, int menuItemId, string comment);
    void IncreaseQuantity(int orderId, int menuItemId);
    void AddOrderItem(int orderId, int menuItemId, string comment);
    Order GetActiveOrderForTable(int tableId);
}



  
