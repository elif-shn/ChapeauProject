using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IFinancialService
    {
        FinancialOverviewResult GetOverview(FinancialPeriod period, DateTime? startDate, DateTime? endDate);
    }
}