using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class RestaurantOverviewViewModel
    {
        public Table Table { get; set; }

        public List<Order> Orders { get; set; } = new();

        public bool HasFoodOrders { get; set; }

        public bool HasDrinkOrders { get; set; }

        public bool FoodReady { get; set; }

        public bool DrinkReady { get; set; }

    }
}
