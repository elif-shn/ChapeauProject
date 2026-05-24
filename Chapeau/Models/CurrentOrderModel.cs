namespace Chapeau.Models
{
    public class CurrentOrderModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
