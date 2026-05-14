namespace Chapeau.ViewModels
{
    public class RunningOrderViewModel
    {
        public int OrderId { get; set; }

        public int TableId { get; set; }

        public DateTime OrderTime { get; set; }

        public string WaitingTime { get; set; }

        public string OrderStatus { get; set; }

        public RunningOrderViewModel(int orderId, int tableId, DateTime orderTime, string waitingTime, string orderStatus)
        {
            OrderId = orderId;
            TableId = tableId;
            OrderTime = orderTime;
            WaitingTime = waitingTime;
            OrderStatus = orderStatus;
        }
    }
}
