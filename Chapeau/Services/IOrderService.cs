using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        Order? GetOrderById(int id);
        void UpdateOrderStatus(int orderId, OrderStatus status);
    }
}
