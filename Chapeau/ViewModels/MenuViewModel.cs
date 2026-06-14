using Chapeau.Enums;
using Chapeau.Models;
namespace Chapeau.ViewModels
{
    public class MenuViewModel
    {
        public List<Menu> Menu { get; set; } = new List<Menu>();
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public MenuItem ItemToEdit { get; set; } = new MenuItem();

    }
}