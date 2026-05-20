using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class TakeOrderViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public List<Category> Categories { get; set; }

        public int? SelectedMenuId { get; set; }
        public Category? SelectedCategory { get; set; }

        public List<Table> Tables { get; set; }
        public int? SelectedTableId { get; set; }
        public int? ActiveOrderId { get; set; }
        public bool IsTakeOrder { get; set; }
        public List<OrderItem> CurrentOrderItems { get; set; } = new List<OrderItem>();
        public int? OrderId { get; set; }
    }
}