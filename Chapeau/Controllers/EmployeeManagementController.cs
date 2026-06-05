using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class EmployeeManagementController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeManagementController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            try
            {
                return View(new EmployeeManagementViewModel
                {
                    Employees = _employeeService.GetAllEmployees()
                });
            }
            catch (Exception ex)
            {
                return View(new EmployeeManagementViewModel
                {
                    Employees = new List<Employee>()
                });
            }
        }

        public IActionResult Add()
        {
            return View(new EmployeeManagementViewModel { EmployeeToEdit = new Employee() });
        }

        [HttpPost]
        public IActionResult Add(EmployeeManagementViewModel model)
        {
            try
            {
                if (model.EmployeeToEdit != null)
                {
                    _employeeService.AddEmployee(model.EmployeeToEdit);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Employee employee = _employeeService.GetEmployeeById(id);
                if (employee == null)
                    return NotFound();

                return View(new EmployeeManagementViewModel { EmployeeToEdit = employee });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(EmployeeManagementViewModel model)
        {
            try
            {
                if (model.EmployeeToEdit != null)
                {
                    _employeeService.UpdateEmployee(model.EmployeeToEdit);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public IActionResult Deactivate(int id)
        {
            _employeeService.DeactivateEmployee(id);
            return RedirectToAction("Index");
        }

        public IActionResult Activate(int id)
        {
            _employeeService.ActivateEmployee(id);
            return RedirectToAction("Index");
        }
    }
}