using Chapeau.Repositories;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IActionResult Index() 
        { 
            List<RunningOrderViewModel> runningOrdersViewModel = _orderRepository.GetRunningOrder();
            return View(runningOrdersViewModel); 
        }
        public ActionResult RunningOrders()
        {
            List<RunningOrderViewModel> runningOrdersViewModel = _orderRepository.GetRunningOrder();

            return View();
        }
    }
}























