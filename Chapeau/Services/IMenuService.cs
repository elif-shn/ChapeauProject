using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
namespace Chapeau.Services
{
    public interface IMenuService
    {
        /* List<MenuItem> GetAllMenuItems();
         List<MenuItem> GetFilteredMenuItems(int menuId, int category);*/
        List<MenuItem> GetAllByFilter(int? menuId, Category? category);
        MenuItem GetMenuItemById(int id);
        void AddMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
        void ActivateMenuItem(int id);
        void DeactivateMenuItem(int id);
        List<MenuItem> GetActiveItems(int? menuId, Category? category);

    }
}