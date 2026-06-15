using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class RestaurantOverviewViewModel
    {
        public Table Table { get; set; }

        public bool HasFoodOrders { get; set; }

        public bool HasDrinkOrders { get; set; }
    }
}
