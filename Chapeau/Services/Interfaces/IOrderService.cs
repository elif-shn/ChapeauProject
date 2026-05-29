using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<Order> GetFinishedOrders();
        Order? GetOrderById(int id);
        void UpdateOrderStatus(int orderId, OrderStatus status);
        void UpdateOrderItemStatus(int orderItemId, OrderStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void AddOrderItemToOrder(OrderItem newOrderItem);
        List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change);
        List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId);
    }
}