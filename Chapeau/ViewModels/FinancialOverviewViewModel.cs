using Chapeau.Models;
using Chapeau.Enums;

namespace Chapeau.ViewModels
{
    public class FinancialOverviewViewModel
    {
        public FinancialSummary Summary { get; set; } = new FinancialSummary();
        public FinancialPeriod Period { get; set; } = FinancialPeriod.Month;
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}