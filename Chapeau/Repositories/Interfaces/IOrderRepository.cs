using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{
    List<Order> GetRunningOrders(bool isFood);
    List<Order> GetFinishedOrders(bool isFood);
    void UpdateOrderStatus(Order order);
    void UpdateOrderItemStatus(OrderItem orderItem);
    void UpdateCourseStatus(Order order, Category category, OrderItemStatus status);
    Order? GetOrderById(Order order);
    void CreateOrderWithItems(Order order);
    void AddItemsToExistingOrder(Order order, List<OrderItem> items);

    Order? GetActiveOrderForTable(int tableId);
    
    Order? GetRunningTableOrder(int tableId);
    
    Order? GetActiveFoodOrDrinkOrder (int tableId, int isFood);
    List<OrderItem> GetOrderItemsByOrderId(int orderId, bool isFood);
    List<OrderItem> GetOrderItemsByOrderIdNoFilter(Order order);
    void MarkOrderAsServed(int orderId);
    List<Order> GetRunningTableOrders(int tableId);
}



