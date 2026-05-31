using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        List<Menu> GetMenus(Card? card, Category? category, bool onlyActive);
        MenuItem GetById(int id);
        void Add(MenuItem item, int selectedCard, int selectedCategory);
        void Update(MenuItem item, int selectedCard, int selectedCategory);
        void SetActive(int id, bool isActive);
        void DecreaseStock(int menuItemId, int amount);
    }
}