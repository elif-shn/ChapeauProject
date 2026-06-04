using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStockService
    {
        List<Menu> GetAllByFilter(Card? card, Category? category);
        List<Category> GetCategoriesByCard(List<Menu> menus, Card? selectedCard);
        void UpdateStock(int menuItemId, int newStock);
    }
}