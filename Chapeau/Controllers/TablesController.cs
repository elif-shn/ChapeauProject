using Chapeau.Enums;
using Chapeau.Models;
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
                    RestaurantOverviewViewModel tableOverview = new RestaurantOverviewViewModel()
                    {
                        Table = table,
                        HasFoodOrders = _orderServices.GetActiveFoodOrDrinkOrder(table.TableId, 1) != null,
                        HasDrinkOrders = _orderServices.GetActiveFoodOrDrinkOrder(table.TableId, 0) != null,
                        Order  = _orderServices.GetRunningTableOrder(table.TableId)
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
            try
            {
                Order order = _orderServices.GetRunningTableOrder(tableId);

                List<Order> orders = new List<Order>();
                if (order != null)
                {
                    orders.Add(order);
                }

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpPost]
        public IActionResult MarkServed(int orderId, int tableId)
        {
            try
            {
                _orderServices.MarkOrderAsServed(orderId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ShowOrders", new { tableId });
        }

        [HttpPost]
        public IActionResult ChangeTableStatus(Table table)
        {
            try
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
//test
//test 2
//test 3