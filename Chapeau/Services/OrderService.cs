using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public List<Order> GetAllOrders()
    {
        return _orderRepository.GetAllOrders();
    }

    public Order? GetOrderById(int id)
    {
        return _orderRepository.GetOrderById(id);
    }

    public void UpdateOrderStatus(int orderId, OrderStatus status)
    {
        _orderRepository.UpdateOrderStatus(orderId, status);
    }
    public List<Order> GetFinishedOrders()
    {
        return _orderRepository.GetFinishedOrders();
    }

    List<OrderItem> IOrderService.GetOrderItemsByOrderId(int orderId)
    {
        return _orderRepository.GetOrderItemsByOrderId(orderId);
    }
    public void UpdateOrderItemStatus(int orderItemId, OrderStatus status)
    {
        _orderRepository.UpdateOrderItemStatus(orderItemId, status);
    }
    //For Take Order Part
    public Order? GetActiveOrderForTable(int tableId)
    {
        try
        {

            return _orderRepository.GetActiveOrderForTable(tableId);
        }
        catch
        {
            throw;
        }
    }
    public void AddOrderItemToOrder(OrderItem newOrderItem)
    {
        try
        {
            Order activeOrder = GetActiveOrderForTable(newOrderItem.Order.TableId); 
            Order newOrder;

            if (activeOrder == null)
            {
                newOrder = _orderRepository.CreateOrder(newOrderItem.Order.TableId);
            }
            else
            {
                newOrder = activeOrder;
            }

            bool itemExists = _orderRepository.OrderItemExists(newOrderItem);

            if (itemExists)
            {
                _orderRepository.IncreaseOrderItemQuantity(newOrderItem);
            }
            else
            {
                _orderRepository.AddOrderItemToOrder(newOrderItem);
            }
        }
        catch
        {
            throw;
        }
    }
    public List<OrderItem> ModifyCurrentOrderItem(List<OrderItem> currentItems,OrderItem newItem, int change)
    {
        /*if (newItem.MenuItem.StockStatus == StockStatus.OutOfStock)
        {
            throw new Exception("This item is currently out of stock!");
        }*/

        newItem.Comment ??= "";

        foreach (OrderItem item in currentItems)
        {
            if (item.MenuItem.MenuItemId == newItem.MenuItem.MenuItemId && item.Comment == newItem.Comment)
            {
                if (change > 0 && item.OrderItemQuantity + change > newItem.MenuItem.Stock)
                {
                    throw new Exception("Not enough stock available!");
                }

                item.OrderItemQuantity += change;

                if (item.OrderItemQuantity <= 0)
                {
                    currentItems.Remove(item);
                }

                return currentItems;
            }
        }

        if (change > 0)
        {
            newItem.OrderItemQuantity = change;
            currentItems.Add(newItem);
        }

        return currentItems;
    }
    public List<OrderItem> RemoveItem(List<OrderItem> currentItems, int menuItemId)
    {
        try
        {
            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].MenuItem.MenuItemId == menuItemId)
                {
                    currentItems.RemoveAt(i);
                    i--;
                }
            }
            return currentItems;
        }
        catch
        {
            throw;
        }
    }
}