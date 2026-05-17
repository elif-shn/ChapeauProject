using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
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
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
                                   
        {
            User? user =
                _userServices.GetByUsernameAndPassword(
                    username,
                    password);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";

                return View();
            }

            HttpContext.Session.SetObject(
                "LoggedInUser",
                user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");

            return RedirectToAction("Login");
        }
    }
}

