using Chapeau.Services;
using Chapeau.ViewModels;
using Chapeau.Models;
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
                FinancialOverviewResult result = _financialService.GetOverview(period, startDate, endDate);

                return View(new FinancialOverviewViewModel
                {
                    Summary = result.Summary,
                    Period = period,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate
                });
            }
            catch (Exception ex)
            {
                return View(new FinancialOverviewViewModel { Period = period });
            }
        }
    }
}