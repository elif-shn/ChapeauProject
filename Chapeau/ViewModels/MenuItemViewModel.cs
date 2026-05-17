using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuItemViewModel
    {
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public int MenuId { get; set; }
        public Category Category { get; set; }
        public string StockStatus { get; set; }
        public MenuItemViewModel() { }
        public MenuItemViewModel(string menuItemName, decimal menuItemPrice, int menuId, Category category, string stockStatus)
        {
            MenuItemName = menuItemName;
            MenuItemPrice = menuItemPrice;
            MenuId = menuId;
            Category = category;
            StockStatus = stockStatus;
        }
    }
}
