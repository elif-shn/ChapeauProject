using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class TakeOrderViewModel
    {
        public MenuFilterData MenuFilterData { get; set; }
        public int? SelectedTableId { get; set; }
        public List<OrderItem> CurrentOrders { get; set; } = new();
        public OrderItem NewOrderItem { get; set; }

    }
}