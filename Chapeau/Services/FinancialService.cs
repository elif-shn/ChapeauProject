using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IFinancialRepository _financialRepository;

        public FinancialService(IFinancialRepository financialRepository)
        {
            _financialRepository = financialRepository;
        }

        public FinancialOverviewResult GetOverview(string period, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                DateTime start;
                DateTime end = DateTime.Now;

                switch (period)
                {
                    case "month":
                        start = DateTime.Now.AddMonths(-1);
                        break;
                    case "quarter":
                        start = DateTime.Now.AddMonths(-3);
                        break;
                    case "year":
                        start = DateTime.Now.AddYears(-1);
                        break;
                    case "custom":
                        start = startDate ?? DateTime.Now.AddMonths(-1);
                        end = endDate ?? DateTime.Now;
                        break;
                    default:
                        start = DateTime.Now.AddMonths(-1);
                        break;
                }

                return new FinancialOverviewResult
                {
                    Summary = _financialRepository.GetFinancialData(start, end),
                    StartDate = start,
                    EndDate = end
                };
            }
            catch
            {
                throw;
            }
        }
    }
}