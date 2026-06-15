using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {       
        List<Order> GetRunningOrders(bool isFood);
        List<Order> GetFinishedOrders(bool isFood);
        Order? GetOrderById(Order order);
        void UpdateOrderStatus(Order order);
        void UpdateOrderItemStatus(OrderItem orderItem);
        void UpdateCourseStatus(Order order, Category category, OrderItemStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void SendOrder(Order newOrder);
        List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change);
        List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId);
    }
}