using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;
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
    
    public Order?GetOrderById(int id)
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
    public void AddItemToTableOrder(int tableId, int menuItemId, string comment)
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
                _orderRepository.AddOrderItem(orderId, menuItemId, comment);
            }
        }
        catch
        {
            throw;
        }
    }
}