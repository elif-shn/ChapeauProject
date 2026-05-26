using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
namespace Chapeau.Services.Interfaces
{
    public interface IMenuService
    {
        List<Menu> GetAllByFilter(Card? card, Category? category);
        MenuItem GetMenuItemById(int id);
        void AddMenuItem(MenuItem item, int selectedCard, int selectedCategory);
        void UpdateMenuItem(MenuItem item, int selectedCard, int selectedCategory);
        void ActivateMenuItem(int id);
        void DeactivateMenuItem(int id);
        List<Menu> GetActiveItems(Card? card, Category? category);
        List<Category> GetCategoriesByCard(List<Menu> menus, Card? selectedCard);
        (List<Menu> menus, List<Category> categories) GetMenuDisplay(Card? selectedCard, Category? selectedCategory);

        void DecreaseStock(int menuItemId, int amount);
    }
}