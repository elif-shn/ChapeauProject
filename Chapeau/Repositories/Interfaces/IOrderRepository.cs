using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{
    List<Order> GetRunningOrders(bool isFood);
    List<Order> GetFinishedOrders(bool isFood);
    public void UpdateOrderStatus(Order order);
    public void UpdateOrderItemStatus(OrderItem orderItem);
    public void UpdateCourseStatus(Order order, Category category, OrderItemStatus status);
    public Order? GetOrderById(Order order);
    public void CreateOrderWithItems(Order order);
    public void AddItemsToExistingOrder(Order order, List<OrderItem> items);
    public Order GetActiveOrderForTable(int tableId);
}



