using Microsoft.AspNetCore.Mvc;

namespace nomnomnavis.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
