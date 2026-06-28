using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chapeau.Controllers
{
    public class AccountController : Controller
    {
         
        private readonly IEmployeeService _employeeServices;
        
        public AccountController(IEmployeeService employeeServices)
        {
            this._employeeServices = employeeServices;
        }

        //you can Log in as a waiter with username: Marie Lee, password: admin123
        [AllowAnonymous]
        public IActionResult Login()
        {
            LoginModel loginModel = new LoginModel();

            return View(loginModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
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

            HttpContext.Session.SetObject("LoggedInUser", employee);
            

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                new Claim(ClaimTypes.Name, employee.EmployeeName),
                new Claim(ClaimTypes.Role, employee.EmployeeOccupation.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ChapeauCookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync("ChapeauCookie", principal);

            return RedirectToAction("Index", "Home");
            
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");

            HttpContext.SignOutAsync("ChapeauCookie");

            return RedirectToAction("Login");
        }
    }
}