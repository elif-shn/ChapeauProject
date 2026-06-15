using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    [Authorize(Roles = "Manager")]
    public class EmployeeManagementController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeManagementController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        /*for login to the management part user name : Mehedi and Password :12345*/
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
                    TempData["SuccessMessage"] = "Employee added successfully!";
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

                employee.EmployeePassword = ""; // don't expose hashed password to the view

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
                    TempData["SuccessMessage"] = "Employee updated successfully!";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

       
        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            _employeeService.DeactivateEmployee(id);
            TempData["SuccessMessage"] = "Employee deactivated successfully!";
            return RedirectToAction("Index");
        }

      
        [HttpPost]
        public IActionResult Activate(int id)
        {
            _employeeService.ActivateEmployee(id);
            TempData["SuccessMessage"] = "Employee activated successfully!";
            return RedirectToAction("Index");
        }
    }
}