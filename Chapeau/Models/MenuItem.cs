using Chapeau.Enums;
namespace Chapeau.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public int MenuId { get; set; }
        public Category Category { get; set; }
        public int VatPercentage { get; set; }

        public int Stock { get; set; }

        public MenuItem() { }
        public MenuItem(int menuItemId, string menuItemName, decimal menuItemPrice, int menuId, Category category,int vatPercentage, int stock)
        {
            MenuItemId = menuItemId;
            MenuItemName = menuItemName;
            MenuItemPrice = menuItemPrice;
            MenuId = menuId;
            Category = category;
            VatPercentage = vatPercentage;
            Stock = stock;
        }
    }
}
