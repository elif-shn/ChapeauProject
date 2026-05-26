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
        void AddOrderItemToOrder(int tableId, int menuItemId, string comment);
        List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem, MenuItem menuItem);
        List<CurrentOrderModel> UpdateItemQuantity(List<CurrentOrderModel> items, int menuItemId, int change, MenuItem menuItem);
        List<CurrentOrderModel> RemoveItem(List<CurrentOrderModel> items, int menuItemId);
    }
}