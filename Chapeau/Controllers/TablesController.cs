
using Microsoft.AspNetCore.Mvc;
using Chapeau.Services.Interfaces;

namespace Chapeau.Controllers
{
    public class TablesController : Controller
    {
        private readonly ITablesService _tableService;


        public TablesController(ITablesService tableService)

        {
            this._tableService = tableService;

        }

        public IActionResult Index()
        {
            var tables = _tableService.GetAllTables();


            return View(tables);
        }
    }
}
