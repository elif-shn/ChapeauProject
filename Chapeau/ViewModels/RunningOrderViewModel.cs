using Chapeau.Models;
using Chapeau.Extensions;

namespace Chapeau.ViewModels
{
    public class RunningOrderViewModel
    {
        public Order Order { get; set; }

        public Table Table { get; set; }

        public DateTime OrderTime { get; set; }

        public string WaitingTime { get; set; }

        public string OrderStatus { get; set; }

        public RunningOrderViewModel(Order order, Table table, DateTime orderTime, string waitingTime, string orderStatus)
        {
            Order = order;

            Table = table;

            OrderTime = orderTime;

            WaitingTime = waitingTime;

            OrderStatus = orderStatus;
        }

        public RunningOrderViewModel()
        {

        }
    }
}