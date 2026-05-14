using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuService
    {
        List<MenuItem> GetAllMenuItems();
        List<MenuItem> GetFilteredMenuItems(int menuId);
        MenuItem GetMenuItemById(int id);
        void AddMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
    }
}