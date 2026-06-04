using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class FinancialOverviewController : Controller
    {
        private readonly IFinancialService _financialService;

        public FinancialOverviewController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        public IActionResult Index(string period = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var summary = _financialService.GetOverview(period, startDate, endDate);

                DateTime start;
                DateTime end = DateTime.Now;

                switch (period)
                {
                    case "month": start = DateTime.Now.AddMonths(-1); break;
                    case "quarter": start = DateTime.Now.AddMonths(-3); break;
                    case "year": start = DateTime.Now.AddYears(-1); break;
                    case "custom":
                        start = startDate ?? DateTime.Now.AddMonths(-1);
                        end = endDate ?? DateTime.Now;
                        break;
                    default: start = DateTime.Now.AddMonths(-1); break;
                }

                return View(new FinancialOverviewViewModel
                {
                    Summary = summary,
                    Period = period,
                    StartDate = start,
                    EndDate = end
                });
            }
            catch (Exception ex)
            {
                return View(new FinancialOverviewViewModel
                {
                    Summary = new Chapeau.Models.FinancialSummary(),
                    Period = period
                });
            }
        }
    }
}