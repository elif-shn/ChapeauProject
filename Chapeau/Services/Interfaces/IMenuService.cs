using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
using System;

public interface IMenuService
{
    List<Menu> GetMenus(Card? card, Category? category, bool onlyActive);

    MenuItem GetMenuItemById(int id);

    void AddMenuItem(MenuItem item, int selectedCard, int selectedCategory);

    void UpdateMenuItem(MenuItem item, int selectedCard, int selectedCategory);

    void ActivateMenuItem(int id);

    void DeactivateMenuItem(int id);

    void DecreaseStock(int menuItemId, int amount);
}