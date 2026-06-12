
using Chapeau.Enums;
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
            List<ActiveOrderViewModel> orders = _tableService.GetActiveOrders(tableId);
            

            return View(orders);
        }

        [HttpPost]
        public IActionResult MarkServed(int orderId, int tableId)
        {

            _tableService.MarkOrderAsServed(orderId);
                
            return RedirectToAction("ShowOrders", new { tableId });
        }

        [HttpPost]
        public IActionResult ChangeTableStatus(Table table)
        {
            if (table.TableStatus == TableStatus.Free)
            {
                table.TableStatus = TableStatus.Occupied;


                _tableService.UpdateTableStatus(table);
                    
            }
            else
            {
                bool hasActiveOrders = _tableService.HasActiveOrders(table.TableId);
                

                if (!hasActiveOrders)
                {
                    table.TableStatus = TableStatus.Free;


                    _tableService.UpdateTableStatus(table);
                        
                }
                else
                {
                    TempData["Error"] = "Table has active orders.";
                    
                }
            }

            return RedirectToAction("Index");
        }
    }
}
