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
        public List<Table> OccupiedTables { get; set; } = new();
        public int? SelectedTableId { get; set; }
        public List<CurrentOrderModel> CurrentOrder { get; set; } = new();
    }
}