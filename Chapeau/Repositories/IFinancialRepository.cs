using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IFinancialRepository
    {
        FinancialSummary GetFinancialData(DateTime startDate, DateTime endDate);
    }
}