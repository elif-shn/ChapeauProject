using Chapeau.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace Chapeau.Controllers
{
    public class TablesController : Controller
    {
        private readonly ITableRepository tableRepository;


        public TablesController(ITableRepository tableRepository)

        {
            this.tableRepository = tableRepository;

        }

        public IActionResult Index()
        {
            var tables = tableRepository.GetAllTables();


            return View(tables);
        }
    }
}
