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
        public MenuItemViewModel() { }
        public MenuItemViewModel(string menuItemName, decimal menuItemPrice, int menuId, Category category)
        {
            MenuItemName = menuItemName;
            MenuItemPrice = menuItemPrice;
            MenuId = menuId;
            Category = category;
        }
    }
}
