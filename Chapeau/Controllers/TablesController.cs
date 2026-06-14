
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
        private readonly IOrderService _orderServices;

        public TablesController(ITablesService tableService, IOrderService orderService)
        {
            this._tableService = tableService;
            this._orderServices = orderService;
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
            List<Order> orders = _tableService.GetRunningTableOrders(tableId);
            

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
