using Chapeau.Enums;
using Chapeau.Models;
namespace Chapeau.ViewModels
{
    public class MenuViewModel
    {
        public List<Menu> Menu { get; set; }
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Category> Categories { get; set; }
        public MenuItem ItemToEdit { get; set; } = new MenuItem();

    }
}