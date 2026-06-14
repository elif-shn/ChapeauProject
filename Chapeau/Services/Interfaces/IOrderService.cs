using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetRunningOrders();
        List<Order> GetFinishedOrders();
        List<Order> GetKitchenOrders();
        List<Order> GetBarOrders();
        Order? GetOrderById(Order order);
        void UpdateOrderStatus(Order order, OrderStatus status);
        void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void SendOrder(Order newOrder);
        void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void AddNote(List<OrderItem> currentItems,int menuItemId, string comment);
        void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "");
    }
}