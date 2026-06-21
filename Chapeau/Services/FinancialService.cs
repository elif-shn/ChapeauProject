using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Enums;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IFinancialRepository _financialRepository;

        public FinancialService(IFinancialRepository financialRepository)
        {
            _financialRepository = financialRepository;
        }

        public FinancialOverviewResult GetOverview(FinancialPeriod period, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                DateTime start;
                DateTime end = DateTime.Now;

                switch (period)
                {
                    case FinancialPeriod.Month:
                        start = DateTime.Now.AddMonths(-1);
                        break;

                    case FinancialPeriod.Quarter:
                        start = DateTime.Now.AddMonths(-3);
                        break;

                    case FinancialPeriod.Year:
                        start = DateTime.Now.AddYears(-1);
                        break;

                    case FinancialPeriod.Custom:
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