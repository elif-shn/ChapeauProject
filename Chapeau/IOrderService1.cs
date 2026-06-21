using System.Collections.Generic;
using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetRunningOrders(bool isFood);

        List<Order> GetRunningTableOrders(int tableId);

        List<Order> GetFinishedOrders(bool isFood);

        void UpdateOrderStatus(Order order);

        void UpdateOrderItemStatus(OrderItem orderItem);

        void UpdateCourseStatus(Order order, Category category, OrderItemStatus status);

        Order? GetOrderById(Order order);

        Order? GetActiveOrderForTable(int tableId);

        void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");

        void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "");

        void SendOrder(Order newOrder);

        void AddNote(List<OrderItem> currentItems, int menuItemId, string comment);

        void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "");

        void MarkOrderAsServed(int orderId);

        List<Order> GetActiveDrinkOrders(int tableId);

        List<Order> GetActiveFoodOrders(int tableId);
    }
}