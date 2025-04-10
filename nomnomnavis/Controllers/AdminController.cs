using Microsoft.AspNetCore.Mvc;
using nomnomnavis.Models;
using nomnomnavis.Services;

namespace nomnomnavis.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IReviewService _reviewService;

        public AdminController(IRestaurantService restaurantService, IReviewService reviewService)
        {
            _restaurantService = restaurantService;
            _reviewService = reviewService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("role") == "Admin";
        }

        public IActionResult ManageRestaurants()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var restaurants = _restaurantService.GetAll();
            return View(restaurants);
        }

        public IActionResult AddRestaurant()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            return View(new Restaurant()); // ✅ send a fresh Restaurant object to the view
        }


        [HttpPost]
        public IActionResult AddRestaurant(Restaurant restaurant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _restaurantService.Add(restaurant);
            return RedirectToAction("ManageRestaurants");
        }

        public IActionResult EditRestaurant(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var restaurant = _restaurantService.Get(id);
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult EditRestaurant(Restaurant restaurant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _restaurantService.Update(restaurant);
            return RedirectToAction("ManageRestaurants");
        }

        public IActionResult DeleteRestaurant(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _restaurantService.Delete(id);
            return RedirectToAction("ManageRestaurants");
        }

        public IActionResult ModerateReviews()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            var reviews = _reviewService.GetAll();
            return View(reviews);
        }

        public IActionResult DeleteReview(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            _reviewService.Delete(id);
            return RedirectToAction("ModerateReviews");
        }
    }
}
