using Chapeau.Enums;
using Chapeau.Models;
using System.Collections.Generic;

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
        Order? GetActiveOrderForTable(int tableId);
        void AddItemToTableOrder(int tableId, int menuItemId, string comment);
        int CreateOrder(int tableId);
        bool OrderItemExists(int orderId, int menuItemId, string comment);
        void IncreaseQuantity(int orderId, int menuItemId);
        void AddOrderItem(int orderId, int menuItemId, string comment);
    }
}