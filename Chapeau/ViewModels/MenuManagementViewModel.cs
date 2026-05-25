using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuManagementViewModel
    {
        // List of menu items (your Index page uses this)
        public List<Menu> Menu { get; set; }

        // Filtering
        public List<Category> Categories { get; set; }
        public Card? SelectedCard { get; set; }
        public Category? SelectedCategory { get; set; }

        // For Add/Edit dropdowns
        public List<Card> Cards { get; set; }

        // For Add/Edit forms
        public MenuItem ItemToEdit { get; set; }
    }
}
