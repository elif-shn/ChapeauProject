using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ITakeOrderService
    {
        List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem);
    }

}

