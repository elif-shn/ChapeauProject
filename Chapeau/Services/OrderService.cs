using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public List<Order> GetAllOrders() => _orderRepository.GetAllOrders();

    public Order? GetOrderById(int id) => _orderRepository.GetOrderById(id);

    public void UpdateOrderStatus(int orderId, OrderStatus status) => _orderRepository.UpdateOrderStatus(orderId, status);

    public List<Order> GetFinishedOrders() => _orderRepository.GetFinishedOrders();

    public List<OrderItem> GetOrderItemsByOrderId(int orderId) => _orderRepository.GetOrderItemsByOrderId(orderId);

    public void UpdateOrderItemStatus(int orderItemId, OrderStatus status) => _orderRepository.UpdateOrderItemStatus(orderItemId, status);

    public Order? GetActiveOrderForTable(int tableId) => _orderRepository.GetActiveOrderForTable(tableId);

    public void AddItemToTableOrder(int tableId, int menuItemId, string comment)
    {
        Order activeOrder = GetActiveOrderForTable(tableId);
        int orderId = (activeOrder == null) ? _orderRepository.CreateOrder(tableId) : activeOrder.OrderId;

        if (_orderRepository.OrderItemExists(orderId, menuItemId, comment))
        {
            _orderRepository.IncreaseQuantity(orderId, menuItemId);
        }
        else
        {
            _orderRepository.AddOrderItem(orderId, menuItemId, comment);
        }
    }
    public int CreateOrder(int tableId) => _orderRepository.CreateOrder(tableId);

    public bool OrderItemExists(int orderId, int menuItemId, string comment) => _orderRepository.OrderItemExists(orderId, menuItemId, comment);

    public void IncreaseQuantity(int orderId, int menuItemId) => _orderRepository.IncreaseQuantity(orderId, menuItemId);

    public void AddOrderItem(int orderId, int menuItemId, string comment) => _orderRepository.AddOrderItem(orderId, menuItemId, comment);
}