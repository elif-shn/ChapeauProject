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
    public void AddItemToCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "")
    {
        OrderItem ?existingItem = currentOrder.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId &&
        (i.Comment ?? "") == (comment ?? "")
    );
        MenuItem menuItem = _menuRepository.GetById(menuItemId);

        if (existingItem != null)
        {

            if (existingItem.OrderItemQuantity >= menuItem.Stock)
            {
                throw new Exception("Not enough stock available.");
            }

            existingItem.Increase();
            return;
        }
            
        if(menuItem.Stock <= 0)
        {
            throw new Exception("Not enough stock available.");
        } 
        currentOrder.Add(new OrderItem
        {
            MenuItem = menuItem,
            OrderItemQuantity = 1,
            Comment = comment ?? "",
            OrderItemStatus = OrderItemStatus.Ordered
        });           
    }

    public void DecreaseItemQuantityInCurrentOrder(List<OrderItem> currentOrder, int menuItemId, string comment = "")
    {
        OrderItem ?existingItem = currentOrder.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId &&
        (i.Comment ?? "") == (comment ?? ""));

        if (existingItem == null)
            return;

        existingItem.Decrease();

        if (existingItem.OrderItemQuantity <= 0)
        {
            currentOrder.Remove(existingItem);
        }
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
    }
    public void AddNote(List<OrderItem> currentItems, int menuItemId, string comment)
    {
        OrderItem item = currentItems.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId);

        if (item != null)
        {
            item.Comment = comment;
        }
    }
    public void DeleteItem(List<OrderItem> currentItems, int menuItemId, string comment = "")
    {
        currentItems?.RemoveAll(item => item.MenuItem.MenuItemId == menuItemId &&
        (item.Comment ?? "") == (comment ?? ""));
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