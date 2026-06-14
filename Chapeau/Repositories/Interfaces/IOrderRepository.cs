using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{
    List<Order> GetRunningOrders();

    List<Order> GetRunningTableOrders(int tableId);
    List<Order> GetActiveDrinkOrders(int tableId);
    List<Order> GetActiveFoodOrders(int tableId);
    List<OrderItem> GetOrderItemsByOrderId(Order order);
    List<Order> GetFinishedOrders();
    void UpdateOrderStatus(Order order, OrderStatus status);
    void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status);
    Order? GetOrderById(Order order);
    public void CreateOrderWithItems(Order order);
    void AddItemsToExistingOrder(Order order, List<OrderItem> items);
    Order GetActiveOrderForTable(int tableId);
    void MarkOrderAsServed(int orderId);
}



