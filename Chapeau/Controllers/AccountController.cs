using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class AccountController : Controller
    {
         
        private readonly IUserService _userServices;

        public AccountController(IUserService userServices)
        {
            this._userServices = userServices;
        }

        public IActionResult Login()
        {
            LoginModel loginModel =
                new LoginModel();

            return View(loginModel);
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            User? user =
                _userServices.GetByUsernameAndPassword(
                    loginModel.Username,
                    loginModel.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";

                return View(loginModel);
            }

            HttpContext.Session.SetObject(
                "LoggedInUser",
                user);

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

