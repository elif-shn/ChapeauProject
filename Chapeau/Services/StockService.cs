using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public List<Menu> GetAllByFilter(Card? card, Category? category)
        {
            try
            {
                return _stockRepository.GetAllByFilter(card, category);
            }
            catch
            {
                throw;
            }
        }

        public List<Category> GetCategoriesByCard(List<Menu> menus, Card? selectedCard)
        {
            try
            {
                List<Category> categories = new List<Category>();

                foreach (var menu in menus)
                {
                    if (selectedCard == null || menu.Card == selectedCard)
                    {
                        if (!categories.Contains(menu.Category))
                        {
                            categories.Add(menu.Category);
                        }
                    }
                }

                return categories;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateStock(int menuItemId, int newStock)
        {
            try
            {
                _stockRepository.UpdateStock(menuItemId, newStock);
            }
            catch
            {
                throw;
            }
        }
    }
}