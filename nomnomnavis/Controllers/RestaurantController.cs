using Microsoft.AspNetCore.Mvc;

namespace nomnomnavis.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
