using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetRunningOrders(bool isFood);
        List<Order>? GetRunningTableOrders(int tableId);
        List<Order> GetFinishedOrders(bool isFood);

        void UpdateOrderStatus(Order order, bool isFood);
        void UpdateOrderItemStatus(OrderItem orderItem, Order order, bool isFood);
        void UpdateCourseStatus(Order order, Category category, OrderItemStatus status, bool isFood);

        Order? GetOrderById(Order order);
        Order? GetActiveOrderForTable(int tableId);
        Order? GetActiveFoodOrDrinkOrder(int tableId, int isFood);

        void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void AddNote(List<OrderItem> currentItems, int menuItemId, string comment);
        void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "");

        void SendOrder(Order newOrder);
        void MarkFoodOrDrinkAsServed(int orderId, bool isFood);
        void MarkOrderAsServed(int orderId);
        void CreateOrderWithItems(Order order);
        void AddItemsToExistingOrder(Order order, List<OrderItem> items);

        List<OrderItem> GetOrderItemsByOrderId(int orderId, bool isFood);
        List<OrderItem> GetOrderItemsByOrderIdNoFilter(Order order);
    }
}