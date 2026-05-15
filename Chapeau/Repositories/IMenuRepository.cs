using Chapeau.Models;
namespace Chapeau.Repositories
{
    public interface IMenuRepository
    {
        List<MenuItem> GetAll();
        List<MenuItem> GetByFilter(int menuId, int category);
        MenuItem GetById(int id);
        void Add(MenuItem item);
        void Update(MenuItem item);
        void SetActive(int id, bool isActive);
    }
}