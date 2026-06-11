using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuListViewModel
    {
        public MenuFilterData MenuFilterData { get; set; }
        public bool ShowAddButton { get; set; }

        public int? SelectedTableId { get; set; }
    }
}