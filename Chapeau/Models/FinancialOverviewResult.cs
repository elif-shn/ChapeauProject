namespace Chapeau.Models
{
    public class FinancialOverviewResult
    {
        public FinancialSummary Summary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}