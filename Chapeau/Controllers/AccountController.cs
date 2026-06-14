using Chapeau.Extensions;
using Chapeau.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ChapeauCookie");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("ChapeauCookie", principal);

            return RedirectToAction(
                "Index",
                "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");

            await HttpContext.SignOutAsync("ChapeauCookie");

            return RedirectToAction("Login");
        }
    }
}