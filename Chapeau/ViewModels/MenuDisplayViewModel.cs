using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuDisplayViewModel
    {
        public List<Menu> Menu { get; set; }
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }

    }
}
