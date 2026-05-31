using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetRunningOrders()
        {
            return _orderRepository.GetRunningOrders();
        }

        public List<Order> GetFinishedOrders()
        {
            return _orderRepository.GetFinishedOrders();
        }

        public Order? GetOrderById(int id)
        {
            return _orderRepository.GetOrderById(id);
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return _orderRepository.GetOrderItemsByOrderId(orderId);
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

        public void AddItemToTableOrder(int tableId, int menuItemId, string comment)
        {
            Order? activeOrder = GetActiveOrderForTable(tableId);

            int orderId;

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
    }
}