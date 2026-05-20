using Chapeau.Models;    
namespace Chapeau.Repositories
{
    public interface IOrderItem
    {
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
    }

    }



