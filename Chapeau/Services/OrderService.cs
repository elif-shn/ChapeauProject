using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuService _menuService;

        public OrderService(IOrderRepository orderRepository, IMenuService menuService)
        {
            _orderRepository = orderRepository;
            _menuService = menuService;
        }

        public List<Order> GetRunningOrders(bool isFood)
        {
            return _orderRepository.GetRunningOrders(isFood);
        }

        public List<Order> GetFinishedOrders(bool isFood)
        {
            return _orderRepository.GetFinishedOrders(isFood);
        }

        public void UpdateOrderStatus(Order order)
        {
            _orderRepository.UpdateOrderStatus(order);
        }

        public void UpdateOrderItemStatus(OrderItem orderItem)
        {
            _orderRepository.UpdateOrderItemStatus(orderItem);
        }

        public void UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            _orderRepository.UpdateCourseStatus(order, category, status);
        }

        public Order? GetOrderById(Order order)
        {
            return _orderRepository.GetOrderById(order);
        }
        public Order? GetActiveOrderForTable(int tableId)
    {
        return _orderRepository.GetActiveOrderForTable(tableId);
    }

    public void SendOrder(Order newOrder)
    {
        Order? activeOrder = GetActiveOrderForTable(newOrder.TableId);

        if (activeOrder == null)
        {
            _orderRepository.CreateOrderWithItems(newOrder);
        }
        else
        {
            _orderRepository.AddItemsToExistingOrder(activeOrder, newOrder.OrderItems);
        }
        foreach (OrderItem item in newOrder.OrderItems)
        {
            _menuService.DecreaseStock(item.MenuItem.MenuItemId, item.OrderItemQuantity);
        }
    }

    public List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change)
    {
        OrderItem? existingItem = currentItems.FirstOrDefault(item =>
        item.MenuItem.MenuItemId == newItem.MenuItem.MenuItemId);

        if (change > 0)
        {
            int currentQuantityInCart = existingItem?.OrderItemQuantity ?? 0;
            if (currentQuantityInCart + change > newItem.MenuItem.Stock)
                throw new Exception("Not enough stock available!");
        }

        if (existingItem != null)
        {
            existingItem.OrderItemQuantity += change;

            if (!string.IsNullOrEmpty(newItem.Comment))
            {
                existingItem.Comment = newItem.Comment;
            }

            if (existingItem.OrderItemQuantity <= 0)
                currentItems.Remove(existingItem);
        }
        else if (change > 0)
        {
            newItem.OrderItemQuantity = change;
            newItem.Comment ??= "";
            currentItems.Add(newItem);
        }

        return currentItems;
    }

    public List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId)
    {
        if (currentItems != null)
            currentItems.RemoveAll(item => item.MenuItem.MenuItemId == menuItemId);

        return  currentItems;
    }

       
    }
}




