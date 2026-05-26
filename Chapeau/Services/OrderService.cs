using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    public void AddOrderItemToOrder(int tableId, int menuItemId, string comment)
    {
        try
        {
            Order activeOrder = GetActiveOrderForTable(tableId); int orderId;

            if (activeOrder == null)
            {
                orderId = _orderRepository.CreateOrder(tableId);
            }
            else
            {
                orderId = activeOrder.OrderId;
            }

            bool itemExists = _orderRepository.OrderItemExists(orderId, menuItemId, comment);

            if (itemExists)
            {
                _orderRepository.IncreaseQuantity(orderId, menuItemId);
            }
            else
            {
                _orderRepository.AddOrderItemToOrder(orderId, menuItemId, comment);
            }
        }
        catch
        {
            throw;
        }
    }
    public List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem, MenuItem menuItem)
    {

        try
        {
            if (menuItem.StockStatus == StockStatus.OutOfStock)
            {
                throw new Exception("This item is currently out of stock!");
            }
            string comment = newItem.Comment ?? "";
            CurrentOrderModel existing = null;

            foreach (var item in currentItems)
            {
                if (item.MenuItemId == newItem.MenuItemId && item.Comment == comment)
                {
                    existing = item;
                    break;
                }
            }

            if (existing != null)
            {
                if (existing.Quantity + 1 > menuItem.Stock)
                {
                    throw new Exception("Not enough stock available!");
                }
                existing.Quantity++;
            }
            else
            {
                newItem.Comment = comment;
                newItem.Quantity = 1;
                currentItems.Add(newItem);
            }

            return currentItems;
        }
        catch
        {
            throw;
        }

    }
    public List<CurrentOrderModel> UpdateItemQuantity(List<CurrentOrderModel> items, int menuItemId, int change, MenuItem menuItem)
    {
        CurrentOrderModel itemToUpdate = null;
        try
        {
            foreach (var item in items)
            {
                if (item.MenuItemId == menuItemId)
                {
                    itemToUpdate = item;
                    break;
                }
            }

            if (itemToUpdate != null)
            {
                if (change > 0 && (itemToUpdate.Quantity + 1) > menuItem.Stock)
                {
                    throw new Exception("Not enough stock available!");
                }

                itemToUpdate.Quantity += change;

                if (itemToUpdate.Quantity <= 0)
                {
                    items.Remove(itemToUpdate);
                }
            }
            return items;
        }
        catch
        {
            throw;
        }

    }

    public List<CurrentOrderModel> RemoveItem(List<CurrentOrderModel> items, int menuItemId)
    {
        try
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].MenuItemId == menuItemId)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }
            return items;
        }
        catch
        {
            throw;
        }
    }
}