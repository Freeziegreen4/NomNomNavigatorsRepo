using Microsoft.AspNetCore.Mvc;
using nomnomnavis.Models;
using nomnomnavis.Services;

namespace nomnomnavis.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService) => _userService = userService;

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(User login)
        {
            var user = _userService.Authenticate(login.Username, login.Password);
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            _userService.Register(user);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
