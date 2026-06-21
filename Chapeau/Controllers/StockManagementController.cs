using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.AspNetCore.Authorization;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Services.Interfaces;

namespace Chapeau.Controllers
{
    public class StockManagementController : Controller
    {
        private readonly IStockService _stockService;

        /*for login to the management part user name : Mehedi and Password :12345*/
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

        [Authorize(Roles = "Manager")]

        [HttpPost]
        public IActionResult UpdateStock(int menuItemId, int newStock, Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                _stockService.UpdateStock(menuItemId, newStock);
                TempData["SuccessMessage"] = "Stock updated successfully!";
                return RedirectToAction("Index", new { selectedCard, selectedCategory });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}