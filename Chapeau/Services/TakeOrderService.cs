using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Services
{
    public class TakeOrderService : ITakeOrderService
    {
       
        public List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem)
        {
            string comment = newItem.Comment ?? "";
            CurrentOrderModel existing = null;

            foreach (var item in currentItems)
            {
                if (item.MenuItemId == newItem.MenuItemId && item.Comment == comment)
                {
                    existing = item;
                    break; 
                }
            }

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                newItem.Comment = comment;
                newItem.Quantity = 1;
                currentItems.Add(newItem);
            }

            return currentItems;
        }
    }
}