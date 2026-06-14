using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class AccountController : Controller
    {
         
        private readonly IEmployeeService _employeeServices;
        
        public AccountController(IEmployeeService employeeServices)
        {
            this._employeeServices = employeeServices;
        }

        public IActionResult Login()
        {
            LoginModel loginModel = new LoginModel();
            

            return View(loginModel);
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            Employee? employee =
                _employeeServices.GetByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password);

            if (employee == null)
            {
                ViewBag.Error = "Invalid credentials";

                return View(loginModel);
            }

            HttpContext.Session.SetObject(
                "LoggedInUser",
                employee);

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");

            return RedirectToAction("Login");
        }
    }
}

