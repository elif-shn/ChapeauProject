using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IOrderServices _orderServices;
         
        public OrderController(IOrderServices orderServices )
        {
            _orderServices = orderServices;
        }
        public IActionResult Index() 
        { 
            List<RunningOrderViewModel> runningOrdersViewModel = _orderServices.GetRunningOrders();
            return View(runningOrdersViewModel); 
        }
        public ActionResult RunningOrders()
        {
            List<RunningOrderViewModel> runningOrdersViewModel = _orderServices.GetRunningOrders();

            return View();
        }
    }
}























