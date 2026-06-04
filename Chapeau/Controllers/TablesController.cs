
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class TablesController : Controller
    {
        private readonly ITablesService _tableService;


        public TablesController(ITablesService tableService)

        {
            this._tableService = tableService;

        }

        public IActionResult Index()
        {
            List<Table> tables = new List<Table>();


            tables = _tableService.GetAllTables();
            
            
            return View(tables);
        }

        [HttpGet]
        public IActionResult ShowOrders(int tableId)
        {
            List<ActiveOrderViewModel> orders =
                _tableService
                    .GetActiveOrders(tableId);

            return View(orders);
        }

        [HttpPost]
        public IActionResult MarkServed(int orderId, int tableId)
        {

            _tableService.MarkOrderAsServed(orderId);
                
            return RedirectToAction("ShowOrders", new { tableId });
        }
    }
}
