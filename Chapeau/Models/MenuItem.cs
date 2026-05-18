using Chapeau.Enums;
namespace Chapeau.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public decimal MenuItemPrice { get; set; }
        public Menu Menu { get; set; }
        public Category Category { get; set; }
        public int VatPercentage { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public bool IsHighVat => VatPercentage == 21;
        public string StockStatus
        {
            get
            {
                if (Stock == 0) return "OUT OF STOCK";
                if (Stock <= 10) return "ALMOST OUT OF STOCK";
                return "IN STOCK";
            }
        }

        public string StockCssClass
        {
            get
            {
                return StockStatus switch
                {
                    "OUT OF STOCK" => "text-danger",
                    "ALMOST OUT OF STOCK" => "text-warning",
                    _ => "text-success"
                };
            }
        }

        public MenuItem() { }

        public MenuItem(int menuItemId, string menuItemName, decimal menuItemPrice, Menu menu, Category category, int vatPercentage, int stock,bool isActive)
        {
            MenuItemId = menuItemId;
            MenuItemName = menuItemName;
            MenuItemPrice = menuItemPrice;
            Menu = menu;
            Category = category;
            VatPercentage = vatPercentage;
            Stock = stock;
            IsActive = isActive;
        }
    }
}