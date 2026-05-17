using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderServices
    {
        List<RunningOrderViewModel> GetRunningOrders();
    }
}
