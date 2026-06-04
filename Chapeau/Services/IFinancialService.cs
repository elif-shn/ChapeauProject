using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IFinancialService
    {
        FinancialSummary GetOverview(string period, DateTime? startDate, DateTime? endDate);
    }
}