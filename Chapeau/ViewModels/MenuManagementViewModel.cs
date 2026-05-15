using Chapeau.Models;
namespace Chapeau.ViewModels
{
    public class MenuManagementViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public int SelectedMenuId { get; set; } = 0;
        public int SelectedCategory { get; set; } = 0;
        public MenuItem ItemToEdit { get; set; } = new MenuItem();
    }
}