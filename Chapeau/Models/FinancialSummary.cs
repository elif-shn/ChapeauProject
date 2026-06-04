namespace Chapeau.Models
{
    public class FinancialSummary
    {
        public decimal TotalIncomeLunch { get; set; }
        public decimal TotalIncomeDinner { get; set; }
        public decimal TotalIncomeDrinks { get; set; }
        public int TotalSalesLunch { get; set; }
        public int TotalSalesDinner { get; set; }
        public int TotalSalesDrinks { get; set; }
        public decimal TotalTip { get; set; }

       
        public decimal TotalIncome => TotalIncomeLunch + TotalIncomeDinner + TotalIncomeDrinks;
        public int TotalSales => TotalSalesLunch + TotalSalesDinner + TotalSalesDrinks;
    }
}