using Chapeau.Enums;
using Chapeau.Models;

public class MenuItem
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public decimal MenuItemPrice { get; set; }

    public int MenuId { get; set; }

    public int VatPercentage { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    public StockStatus StockStatus
    {
        get
        {
            if (Stock <= 0)
            {
                return StockStatus.OutOfStock;
            }

            if (Stock <= 10)
            {
                return StockStatus.AlmostOutOfStock;
            }

            return StockStatus.InStock;
        }
    }
}
