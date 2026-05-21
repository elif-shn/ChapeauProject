using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
namespace Chapeau.Repositories
{
    public interface IMenuRepository
    {
        List<Menu> GetAllByFilter(Card? Card, Category? category);
        MenuItem GetById(int id);
        void Add(MenuItem item);
        void Update(MenuItem item);
        void SetActive(int id, bool isActive);
    }
}
