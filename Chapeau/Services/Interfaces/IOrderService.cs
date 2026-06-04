using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetRunningOrders();
        List<Order> GetFinishedOrders();
        Order? GetOrderById(Order order);
        void UpdateOrderStatus(Order order, OrderStatus status);
        void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void SendOrder(Order newOrder);
        List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change);
        List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId);
    }
}