namespace Chapeau.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            string display = value.ToString();
            if (display == "CoffeeAndTea") return "Coffee & Tea";
            if (display == "SoftDrinks") return "Soft Drinks";
            if (display == "SpiritDrinks") return "Spirit Drinks";
            return display;
        }
    }
}
