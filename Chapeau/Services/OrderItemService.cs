using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItem _orderItemRepository;

        public OrderItemService(IOrderItem orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return _orderItemRepository.GetOrderItemsByOrderId(orderId);
        }
    }
}