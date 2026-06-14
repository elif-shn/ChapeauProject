using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }
    public MenuFilterData GetMenuData(Card? selectedCard, Category? selectedCategory, bool onlyActive)
    {
        List<Menu> menusForCategories = _menuRepository
            .GetMenus(selectedCard, null, onlyActive)
            .ToList();

        List<Category> categories = menusForCategories
            .Select(m => m.Category)
            .Distinct()
            .ToList();

        if (selectedCategory != null && !categories.Contains(selectedCategory.Value))
        {
            selectedCategory = null;
        }

        List<Menu> filteredMenus = _menuRepository
            .GetMenus(selectedCard, selectedCategory, onlyActive)
            .ToList();

        return new MenuFilterData
        {
            Menus = filteredMenus,
            Categories = categories,
            SelectedCard = selectedCard,
            SelectedCategory = selectedCategory
        };
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
}