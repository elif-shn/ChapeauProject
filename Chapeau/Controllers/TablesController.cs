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
                    List<Order> orders = _orderServices.GetRunningTableOrders(table.TableId);
                    RestaurantOverviewViewModel tableOverview = new RestaurantOverviewViewModel()

                    {
                        Table = table,

                        Orders = orders,

                        HasFoodOrders =
                orders.Any(o =>
                    o.OrderItems.Any(i => i.MenuItem.IsFood)),

                        HasDrinkOrders =
                orders.Any(o =>
                    o.OrderItems.Any(i => !i.MenuItem.IsFood)),

                        FoodReady =
                orders.Any(o =>
                    o.OrderItems.Any(i =>
                        i.MenuItem.IsFood &&
                        i.OrderItemStatus == OrderItemStatus.Ready)),

                        DrinkReady =
                orders.Any(o =>
                    o.OrderItems.Any(i =>
                        !i.MenuItem.IsFood &&
                        i.OrderItemStatus == OrderItemStatus.Ready))
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
                List<Order> orders = new List<Order>();

                orders = _orderServices.GetRunningTableOrders(tableId);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }


        }

        [HttpPost]
        public IActionResult MarkFoodOrDrinkServed(int orderId, bool isFood)
        {
            try
            {
                _orderServices.MarkFoodOrDrinkAsServed(orderId, isFood);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<RestaurantOverviewViewModel>());
            }

        }

        [HttpPost]
        public IActionResult MarkServed(int orderId, int tableId)
        {
            try
            {
                _orderServices.MarkOrderAsServed(orderId);

                return RedirectToAction("ShowOrders", new { tableId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }

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

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<RestaurantOverviewViewModel>());
            }

        }


    }
}
//test