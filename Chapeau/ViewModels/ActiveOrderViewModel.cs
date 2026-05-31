namespace Chapeau.ViewModels
{
    public class ActiveOrderViewModel
    {
        public int OrderId { get; set; }

        public int TableId { get; set; }

        public string OrderType { get; set; } = "";

        public string OrderStatus { get; set; } = "";
    }

}
