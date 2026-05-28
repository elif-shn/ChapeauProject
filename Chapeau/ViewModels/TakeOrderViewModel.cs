using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class TakeOrderViewModel
    {
        public List<Menu> Menu { get; set; } = new();
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Category> Categories { get; set; }
        public int? SelectedTableId { get; set; }
        public List<OrderItem> CurrentOrders { get; set; } = new();
        public OrderItem NewOrderItem { get; set; }

        /*public TakeOrderViewModel(Card? selectedCard, Category? selectedCategory, int? selectedTableId)
        { 
            SelectedCard = selectedCard;
            SelectedCategory = selectedCategory;    
            SelectedTableId = selectedTableId;
        }*/
    }
}