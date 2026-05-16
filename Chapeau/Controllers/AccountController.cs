using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Extensions;

namespace Chapeau.Controllers
{
    public class AccountController : Controller
    {
        // Fake users for testing
        private static List<User> users = new List<User>()
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Password = "1234",
                Role = "Admin"
            },

            new User
            {
                Id = 2,
                Username = "customer",
                Password = "1234",
                Role = "Customer"
            }
        };

        // GET
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Login(string username,
                                   string password)
        {
            User? user = users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username/password";
                return View();
            }

            // STORE USER IN SESSION
            HttpContext.Session.SetObject("LoggedInUser", user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");

            return RedirectToAction("Login");
        }
    }
}

