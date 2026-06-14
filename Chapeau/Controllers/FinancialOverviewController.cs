using Chapeau.Services;
using Chapeau.ViewModels;
using Chapeau.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Enums;

namespace Chapeau.Controllers
{

    [Authorize(Roles = "Manager")]
    public class FinancialOverviewController : Controller
    {
        private readonly IFinancialService _financialService;

        /*for login to the management part user name : Mehedi and Password :12345*/
        public FinancialOverviewController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        public IActionResult Index(FinancialPeriod period = FinancialPeriod.Month, DateTime? startDate = null, DateTime? endDate = null)
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