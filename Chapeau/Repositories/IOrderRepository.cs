using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
public interface IOrderRepository
{  
    List<Order> GetAllOrders();
    List<OrderItem> GetOrderItemsByOrderId(int orderId);
    void UpdateOrderStatus(int orderId, OrderStatus newStatus);
    Order? GetOrderById(int id);
}



  
