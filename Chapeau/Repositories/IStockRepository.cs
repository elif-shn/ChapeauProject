using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IStockRepository
    {
        List<Menu> GetAllByFilter(Card? card, Category? category);
        void UpdateStock(int menuItemId, int newStock);
    }
}