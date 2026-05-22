using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<Order> GetFinishedOrders();
        Order? GetOrderById(int id);
        void UpdateOrderStatus(int orderId, OrderStatus status);
        void UpdateOrderItemStatus(int orderItemId, OrderStatus status);
    }
}
