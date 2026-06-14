using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuFilterData
    {
        public List<Menu> Menus { get; set; }
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Category> Categories { get; set; }
        public int? SelectedTableId { get; set; }
    }
}
