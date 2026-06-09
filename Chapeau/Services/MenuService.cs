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
    public MenuFilterData GetMenuData(Card? selectedCard, Category? selectedCategory, bool onlyActive)
    {
        var allMenus = GetMenus(selectedCard, null, onlyActive).ToList();

        var categories = allMenus.Select(m => m.Category).Distinct().ToList();

        if (selectedCategory != null && !categories.Contains(selectedCategory.Value))
        {
            selectedCategory = null;
        }

        var filteredMenus = allMenus.Where(m => selectedCategory == null || m.Category == selectedCategory).ToList();

        return new MenuFilterData
        {
            Menus = filteredMenus,
            Categories = categories,
            SelectedCard = selectedCard,
            SelectedCategory = selectedCategory,
        };
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
}