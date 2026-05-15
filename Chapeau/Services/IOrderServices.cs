using Chapeau.ViewModels;

namespace Chapeau.Services
{
        public interface IOrderService
        {
            List<RunningOrderViewModel> GetRunningOrders();
        }
    }

