using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

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

    public Order CreateOrder(int tableId)
    {
        return _orderRepository.CreateOrder(tableId);
    }

    public void SendOrder(Order newOrder)
    {
        Order order = GetActiveOrderForTable(newOrder.TableId) ?? _orderRepository.CreateOrder(newOrder.TableId);
        List<OrderItem> existingItems = _orderRepository.GetOrderItemsByOrderId(order);

        foreach (var item in newOrder.OrderItems)
        {
            string incomingComment = item.Comment?.Trim() ?? "";
            var existingItem = existingItems.FirstOrDefault(oi =>
                oi.MenuItem.MenuItemId == item.MenuItem.MenuItemId &&
                (oi.Comment?.Trim() ?? "") == incomingComment);

            if (existingItem != null)
            {
                existingItem.Order = order;
                _orderRepository.IncreaseOrderItemQuantity(existingItem, item.OrderItemQuantity);
                existingItem.OrderItemQuantity += item.OrderItemQuantity;
            }
            else
            {
                item.Comment = incomingComment;
                item.Order = order;
                _orderRepository.AddOrderItemToOrder(item);
                existingItems.Add(item);
            }

            _menuService.DecreaseStock(item.MenuItem.MenuItemId, item.OrderItemQuantity);
        }
    }

    public List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change)
    {
        newItem.Comment ??= "";
        var existingItem = currentItems.FirstOrDefault(item =>
            item.MenuItem.MenuItemId == newItem.MenuItem.MenuItemId &&
            item.Comment == newItem.Comment);

        if (change > 0)
        {
            int currentQuantityInCart = existingItem?.OrderItemQuantity ?? 0;
            if (currentQuantityInCart + change > newItem.MenuItem.Stock)
                throw new Exception("Not enough stock available!");
        }

        if (existingItem != null)
        {
            existingItem.OrderItemQuantity += change;
            if (existingItem.OrderItemQuantity <= 0)
                currentItems.Remove(existingItem);
        }
        else if (change > 0)
        {
            newItem.OrderItemQuantity = change;
            currentItems.Add(newItem);
        }

        return currentItems;
    }

    public List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId)
    {
        if (currentItems != null)
            currentItems.RemoveAll(item => item.MenuItem.MenuItemId == menuItemId);

        return currentItems;
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