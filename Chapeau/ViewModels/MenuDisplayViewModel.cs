using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuDisplayViewModel
    {
        public List<MenuItem> MenuItems { get; set; }
        public List<Menu> Menus { get; set; }
        public List<Category> Categories { get; set; }

        public int? SelectedMenuId { get; set; }
        public Category? SelectedCategory { get; set; }
    }
}