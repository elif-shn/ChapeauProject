using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface ITakeOrderRepository
    {
        Order GetActiveOrderByTable(int tableId);

        int CreateOrder(int tableId);

        bool OrderItemExists(int orderId, int menuItemId, string comment);

        void IncreaseQuantity(int orderId, int menuItemId);

        void AddOrderItem(int orderId, int menuItemId, string comment);
        void DecreaseStock(int menuItemId, int amount);
    }
}
