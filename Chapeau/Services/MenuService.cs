using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public List<Menu> GetMenus(Card? card, Category? category, bool onlyActive)
    {
        return _menuRepository.GetMenus(card, category, onlyActive);
    }

    public MenuItem GetMenuItemById(int id)
    {
        return _menuRepository.GetById(id);
    }

    public void AddMenuItem(MenuItem item, int selectedCard, int selectedCategory)
    {
        _menuRepository.Add(item, selectedCard, selectedCategory);
    }

    public void UpdateMenuItem(MenuItem item, int selectedCard, int selectedCategory)
    {
        _menuRepository.Update(item, selectedCard, selectedCategory);
    }

    public void ActivateMenuItem(int id)
    {
        _menuRepository.SetActive(id, true);
    }

    public void DeactivateMenuItem(int id)
    {
        _menuRepository.SetActive(id, false);
    }

    public void DecreaseStock(int menuItemId, int amount)
    {
        _menuRepository.DecreaseStock(menuItemId, amount);
    }

    public List<Category> GetCategoriesByCard(List<Menu> menus, Card? selectedCard)
    {
        // Menü listesi null ise koruma sağlamak için boş liste dönelim
        if (menus == null) return new List<Category>();

        // LINQ: Menülerin içinden sadece Category değerlerini seç ve benzersiz (tekrarsız) olanları listele
        return menus.Where(menu => selectedCard == null || menu.Card == selectedCard)
            .Select(menu => menu.Category)
            .Distinct()
            .ToList();
    }
}