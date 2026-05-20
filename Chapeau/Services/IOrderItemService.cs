using Chapeau.Models;
namespace Chapeau.Services
{
        public interface IOrderItemService
        {
            List<OrderItem> GetOrderItemsByOrderId(int orderId);
        }
    }


