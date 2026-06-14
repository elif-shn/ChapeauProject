using Chapeau.Enums;

namespace Chapeau.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            return value switch
            {
                Category.CoffeeAndTea => "Coffee & Tea",
                Category.SoftDrinks => "Soft Drinks",
                Category.SpiritDrinks => "Spirit Drinks",

                StockStatus.OutOfStock => "Out of stock",
                StockStatus.AlmostOutOfStock => "Almost out of stock",
                StockStatus.InStock => "In stock",

                _ => value.ToString()
            };
        }
    }
}
