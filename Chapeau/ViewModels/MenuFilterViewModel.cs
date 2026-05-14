using Chapeau.Models;
using Chapeau.Enums;

namespace Chapeau.ViewModels
{
    public class MenuFilterViewModel
    {
        public int? SelectedMenuId { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Menu> Menus { get; set; }
        public List<MenuItemViewModel> MenuItems { get; set; }

        public List<Category> Categories { get; set; }

        public MenuFilterViewModel()
        {
            MenuItems = new List<MenuItemViewModel>();
            Categories = new List<Category>();
            Menus = new List<Menu>();
        }
    }
}