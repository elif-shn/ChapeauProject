
using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services;
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
            try
            {
                List<Table> tables = _tableService.GetAllTables();


                List<RestaurantOverviewViewModel> overview = new List<RestaurantOverviewViewModel>();


                foreach (Table table in tables)
                {
                    Order? order = _orderServices.GetRunningTableOrder(table.TableId);
                    RestaurantOverviewViewModel tableOverview = new RestaurantOverviewViewModel()

                    {
                        Table = table,

                        Order = order,

                        HasFoodOrders =
                        order?.OrderItems.Any(item =>
                            item.MenuItem.IsFood) ?? false,

                        HasDrinkOrders =
                        order?.OrderItems.Any(item =>
                            !item.MenuItem.IsFood) ?? false,

                        FoodReady =
                        order?.OrderItems.Any(item =>
                            item.MenuItem.IsFood &&
                            item.OrderItemStatus == OrderItemStatus.Ready) ?? false,

                        DrinkReady =
                        order?.OrderItems.Any(item =>
                            !item.MenuItem.IsFood &&
                            item.OrderItemStatus == OrderItemStatus.Ready) ?? false
                    };

                    overview.Add(tableOverview);
                }

                return View(overview);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<RestaurantOverviewViewModel>());
            }
            
        }

        [HttpGet]
        public IActionResult ShowOrders(int tableId)
        {
            Order? order = _orderServices.GetRunningTableOrder(tableId);
            return View(order);

        }

        [HttpPost]
        public IActionResult MarkFoodOrDrinkServed(int orderId, bool isFood)
        {
            _orderServices.MarkFoodOrDrinkAsServed(orderId, isFood);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkServed(int orderId, int tableId)
        {

            _orderServices.MarkOrderAsServed(orderId);
                
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
//test 2