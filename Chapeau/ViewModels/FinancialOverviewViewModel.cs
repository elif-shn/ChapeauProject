using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class FinancialOverviewViewModel
    {
        public FinancialSummary Summary { get; set; } = new FinancialSummary();
        public string Period { get; set; } = "month";
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}