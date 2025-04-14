using System.Text;
using System.Threading.Tasks;
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
        public AuthController(IUserService userService, IHttpClientFactory httpClient)
        {
            _userService = userService;
            _httpClient = httpClient.CreateClient();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User login)
        {
            //var user = _userService.Authenticate(login.Username, login.Password);
            User user = new User();
            //using (var httpClient = new HttpClient())
            //{
                //StringContent content = new StringContent(
                //    JsonConvert.SerializeObject(login), Encoding.UTF8,
                //    "application/json");
            using (var response = await _httpClient.GetAsync("http://localhost:5018/api/UserAPI"))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    ViewBag.NotFound = response.ReasonPhrase;
                string apiRes = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<List<User>>(apiRes).SingleOrDefault(u => u.Username.Equals(login.Username) && u.Password.Equals(login.Password));
            }
            //}
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }
            else
                ViewBag.Error = string.Empty;

            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register() {
            ViewBag.Invalid = false;
            ViewBag.UsernameUsed = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            //_userService.Register(user);
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(
                        JsonConvert.SerializeObject(user),
                        Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://localhost:5018/api/UserAPI", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        ViewBag.UsernameUsed = true;
                    else ViewBag.UsernameUsed = false;
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        ViewBag.Invalid = true;
                    else ViewBag.Invalid = false;
                }
            }
            if (ViewBag.UsernameUsed || ViewBag.Invalid)
                return View();
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
