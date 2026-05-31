using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{
    List<Order> GetAllOrders();
    List<OrderItem> GetOrderItemsByOrderId(int orderId);
    List<Order> GetFinishedOrders();
    void UpdateOrderStatus(int orderId, OrderStatus newStatus);
    void UpdateOrderItemStatus(int orderItemId, OrderStatus status);
    Order? GetOrderById(int id);
    Order CreateOrder(int tableId);
    void IncreaseOrderItemQuantity(OrderItem newOrderItem, int quantity);
    void AddOrderItemToOrder(OrderItem newOrderItem);
    Order GetActiveOrderForTable(int tableId);
}



