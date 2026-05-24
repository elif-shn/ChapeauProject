using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ITakeOrderService
    {
        List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem, MenuItem menuItem);
        List<CurrentOrderModel> UpdateItemQuantity(List<CurrentOrderModel> items, int menuItemId, int change, MenuItem menuItem);
        List<CurrentOrderModel> RemoveItem(List<CurrentOrderModel> items, int menuItemId);
    }

}

