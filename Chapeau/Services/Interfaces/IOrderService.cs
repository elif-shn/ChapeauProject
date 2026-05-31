using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetRunningOrders();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<Order> GetFinishedOrders();
        Order? GetOrderById(int id);
        void UpdateOrderStatus(Order order, OrderStatus status);
        void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void SendOrder(Order newOrder);
        List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change);
        List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId);
    }
}