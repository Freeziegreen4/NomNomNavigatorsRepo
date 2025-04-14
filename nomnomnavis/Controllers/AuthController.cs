using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using nomnomnavis.Models;
using nomnomnavis.Services;

namespace nomnomnavis.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;
        public AuthController(IUserService userService, HttpClient httpClient)
        {
            _userService = userService;
            _httpClient = httpClient;
        }

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
        public async Task<IActionResult> Register(User user)
        {
            //_userService.Register(user);
            using (var response = await _httpClient.PostAsync("http://localhost:5245/api/UserApi",
                new StringContent(
                    JsonConvert.SerializeObject(user),
                    Encoding.UTF8, "application/json")))
            {
                if(response.StatusCode == System.Net.HttpStatusCode.Conflict) {
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
