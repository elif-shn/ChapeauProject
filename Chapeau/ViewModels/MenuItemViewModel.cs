using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuItemViewModel
    {
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public MenuItemViewModel() { }
        public MenuItemViewModel(string menuItemName, decimal menuItemPrice)
        {
            MenuItemName = menuItemName;
            MenuItemPrice = menuItemPrice;
        }
    }
}
