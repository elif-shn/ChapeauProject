using Chapeau.Enums;
using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Services
{
    public class TakeOrderService : ITakeOrderService
    {
       
        public List<CurrentOrderModel> AddOrUpdateOrderItem(List<CurrentOrderModel> currentItems, CurrentOrderModel newItem, MenuItem menuItem)
        {
            
            try
            {
                if (menuItem.StockStatus == StockStatus.OutOfStock)
                {
                    throw new Exception("This item is currently out of stock!");
                }
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
                    if (existing.Quantity + 1 > menuItem.Stock)
                    {
                        throw new Exception("Not enough stock available!");
                    }
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
            catch
            {
                throw;
            }
            
        }
        public List<CurrentOrderModel> UpdateItemQuantity(List<CurrentOrderModel> items, int menuItemId, int change, MenuItem menuItem)
        {
            CurrentOrderModel itemToUpdate = null;
            try
            {
                foreach (var item in items)
                {
                    if (item.MenuItemId == menuItemId)
                    {
                        itemToUpdate = item;
                        break;
                    }
                }

                if (itemToUpdate != null)
                {
                    if (change > 0 && (itemToUpdate.Quantity + 1) > menuItem.Stock)
                    {
                        throw new Exception("Not enough stock available!");
                    }

                    itemToUpdate.Quantity += change;

                    if (itemToUpdate.Quantity <= 0)
                    {
                        items.Remove(itemToUpdate);
                    }
                }
                return items;
            }
            catch
            {
                throw;
            }
           
        }

        public List<CurrentOrderModel> RemoveItem(List<CurrentOrderModel> items, int menuItemId)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].MenuItemId == menuItemId)
                    {
                        items.RemoveAt(i);
                        i--;
                    }
                }
                return items;
            }
            catch
            {
                throw;
            }
        }
    }
}