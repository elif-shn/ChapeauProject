using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuService _menuService;

    public OrderService(IOrderRepository orderRepository, IMenuService menuService)
    {
        _orderRepository = orderRepository;
        _menuService = menuService;
    }

    public List<Order> GetRunningOrders()
    {
        return _orderRepository.GetRunningOrders();
    }

    public List<Order> GetFinishedOrders()
    {
        return _orderRepository.GetFinishedOrders();
    }
    public Order? GetOrderById(Order order)
    {
        return _orderRepository.GetOrderById(order);
    }

    public void UpdateOrderStatus(Order order, OrderStatus status)
    {
        _orderRepository.UpdateOrderStatus(order, status);
    }

    public void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status)
    {
        _orderRepository.UpdateOrderItemStatus(orderItem, status);
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
    public List<Order> GetRunningOrder()
    {
        return _orderRepository
            .GetRunningOrders()
            .Where(o =>
                o.OrderStatus != OrderStatus.Paid &&
                o.OrderStatus != OrderStatus.Cancelled &&
                o.OrderStatus != OrderStatus.Served &&
                o.OrderStatus != OrderStatus.Settled)
            .ToList();
    }
}