using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using System.Xml.Linq;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuRepository _menuRepository;

    public OrderService(IOrderRepository orderRepository, IMenuRepository menuRepository)
    {
        _orderRepository = orderRepository;
        _menuRepository = menuRepository;
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
        order.OrderStatus = status;

        if (status == OrderStatus.Served)
        {
            order.ServedTime = DateTime.Now;

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(order);
            foreach (OrderItem item in items)
            {
                _orderRepository.UpdateOrderItemStatus(item, OrderItemStatus.Served);
            }
        }

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
        _orderRepository.CreateOrderWithItems(newOrder);
        /*Order? activeOrder = GetActiveOrderForTable(newOrder.TableId);

        if (activeOrder == null)
        {
            _orderRepository.CreateOrderWithItems(newOrder);
        }
        else
        {
            _orderRepository.AddItemsToExistingOrder(activeOrder, newOrder.OrderItems);
        }*/
    }

    public List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems, OrderItem newItem, int change)
    {
        MenuItem dbItem = _menuRepository.GetById(newItem.MenuItem.MenuItemId);

        OrderItem? existingItem = currentItems.FirstOrDefault(item =>
            item.MenuItem.MenuItemId == newItem.MenuItem.MenuItemId);

        if (change > 0)
        {
            int currentQuantityInCart = existingItem?.OrderItemQuantity ?? 0;

            if (currentQuantityInCart + change > dbItem.Stock)
            {
                throw new Exception("Not enough stock available!");
            }
        }

        if (existingItem != null)
        {
            existingItem.OrderItemQuantity += change;

            if (!string.IsNullOrEmpty(newItem.Comment))
            {
                existingItem.Comment = newItem.Comment;
            }

            if (existingItem.OrderItemQuantity <= 0)
            {
                currentItems.Remove(existingItem);
            }
        }
        else if (change > 0)
        {
            currentItems.Add(new OrderItem
            {
                MenuItem = dbItem,
                OrderItemQuantity = change,
                Comment = newItem.Comment ?? ""
            });
        }

        return currentItems;
    }

    public List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId)
    {
        if (currentItems != null)
            currentItems.RemoveAll(item => item.MenuItem.MenuItemId == menuItemId);

        return  currentItems;
    }
    public List<Order> GetKitchenOrders()
    {
        return _orderRepository.GetRunningOrders()
            .Where(order => order.OrderItems.Any(item =>
                item.MenuItem.Menu.Card == Card.Lunch || item.MenuItem.Menu.Card == Card.Dinner))
            .ToList();
    }

    public List<Order> GetBarOrders()
    {
        return _orderRepository.GetRunningOrders()
            .Where(order => order.OrderItems.Any(item =>
                item.MenuItem.Menu.Card == Card.Drink))
            .ToList();
    }

}