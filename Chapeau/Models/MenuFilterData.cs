using Chapeau.Enums;

namespace Chapeau.Models
{
    public class MenuFilterData
    {
        public List<Menu> Menus { get; set; }
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Category> Categories { get; set; }
    }
}
