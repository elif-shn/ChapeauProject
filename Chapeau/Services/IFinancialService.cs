using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IFinancialService
    {
        FinancialOverviewResult GetOverview(string period, DateTime? startDate, DateTime? endDate);
    }
}