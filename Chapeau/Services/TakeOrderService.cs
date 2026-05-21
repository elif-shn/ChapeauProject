using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class TakeOrderService : ITakeOrderService
    {
        private readonly ITakeOrderRepository _takeOrderRepository;

        public TakeOrderService(ITakeOrderRepository takeOrderRepository)
        {
            _takeOrderRepository = takeOrderRepository;
        }

        public void AddItemToTable(int tableId, int menuItemId, string comment)
        {
            Order order = _takeOrderRepository.GetActiveOrderByTable(tableId);

            if (order == null || order.OrderStatus == "Paid")
            {
                int newOrderId = _takeOrderRepository.CreateOrder(tableId);

                _takeOrderRepository.AddOrderItem(newOrderId,menuItemId,comment);

                return;
            }

            bool exists = _takeOrderRepository.OrderItemExists(order.OrderId,menuItemId,comment);

            if (exists)
            {
                _takeOrderRepository.IncreaseQuantity(order.OrderId,menuItemId);
            }
            else
            {
                _takeOrderRepository.AddOrderItem(order.OrderId,menuItemId,comment);
            }
            _takeOrderRepository.DecreaseStock(menuItemId, 1);
        }
    }
}