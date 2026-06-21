using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {       
        List<Order> GetRunningOrders(bool isFood);
        List<Order> GetFinishedOrders(bool isFood);
        /*
         List<Order> GetKitchenOrders();
         List<Order> GetBarOrders();
        */
        Order? GetRunningTableOrder(int tableId);
        Order? GetActiveFoodOrDrinkOrder(int tableId, int isFood);
        void MarkOrderAsServed(int orderId);
        Order? GetOrderById(Order order);
        void UpdateOrderStatus(Order order);
        void UpdateOrderItemStatus(OrderItem orderItem);
        void UpdateCourseStatus(Order order, Category category, OrderItemStatus status);
        Order? GetActiveOrderForTable(int tableId);
        void SendOrder(Order newOrder);
        void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");
        void AddNote(List<OrderItem> currentItems,int menuItemId, string comment);
        void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "");
    }
}