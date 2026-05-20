using Chapeau.Enums;
using Chapeau.Models;
namespace Chapeau.ViewModels
{
    public class MenuManagementViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public int? SelectedMenuId { get; set; }
        public Category? SelectedCategory { get; set; }
        public List<Menu> Menus { get; set; }
        public List<Category> Categories { get; set; }
        public MenuItem ItemToEdit { get; set; } = new MenuItem();

    }
}