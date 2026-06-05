using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class StockManagementController : Controller
    {
        private readonly IStockService _stockService;

        public StockManagementController(IStockService stockService)
        {
            _stockService = stockService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                List<Menu> menu = _stockService.GetAllByFilter(selectedCard, selectedCategory);
                return View(new StockManagementViewModel
                {
                    Menu = menu,
                    Categories = _stockService.GetCategoriesByCard(menu, selectedCard),
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory
                });
            }
            catch (Exception ex)
            {
                return View(new StockManagementViewModel
                {
                    Menu = new List<Menu>(),
                    Categories = new List<Category>()
                });
            }
        }

        [HttpPost]
        public IActionResult UpdateStock(int menuItemId, int newStock, Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                _stockService.UpdateStock(menuItemId, newStock);
                return RedirectToAction("Index", new { selectedCard, selectedCategory });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}