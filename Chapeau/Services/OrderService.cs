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
            // DB kontrolünde de sadece MenuItemId'ye bakıyoruz
            var existingItem = existingItems.FirstOrDefault(oi =>
                oi.MenuItem.MenuItemId == item.MenuItem.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Order = order;
                if (!string.IsNullOrEmpty(item.Comment))
                {
                    existingItem.Comment = item.Comment?.Trim();
                }
                _orderRepository.IncreaseOrderItemQuantity(existingItem, item.OrderItemQuantity);
                existingItem.OrderItemQuantity += item.OrderItemQuantity;
            }
            else
            {
                item.Comment = item.Comment?.Trim() ?? "";
                item.Order = order;
                _orderRepository.AddOrderItemToOrder(item);
                existingItems.Add(item);
            }

            _menuService.DecreaseStock(item.MenuItem.MenuItemId, item.OrderItemQuantity);
        }
    }

    public List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change)
    {
        var existingItem = currentItems.FirstOrDefault(item =>
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

            // Eğer yeni gelen istekte bir not varsa eskisinin üzerine yazalım/güncelleyelim
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