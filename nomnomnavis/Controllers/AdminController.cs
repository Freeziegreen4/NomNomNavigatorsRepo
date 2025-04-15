using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using nomnomnavis.Models;
using nomnomnavis.Services;

namespace nomnomnavis.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IReviewService _reviewService;
        private readonly HttpClient _httpClient;

        public AdminController(IRestaurantService restaurantService, IReviewService reviewService,
            IHttpClientFactory httpClient)
        {
            _restaurantService = restaurantService;
            _reviewService = reviewService;
            _httpClient = httpClient.CreateClient();
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("role") == "Admin";
        }

        public async Task<IActionResult> ManageRestaurants()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //var restaurants = _restaurantService.GetAll();
            List<Restaurant> restaurants = new List<Restaurant>();
            using(var response = await _httpClient.GetAsync("http://localhost:5018/api/restaurantAPI"))
            {
                string apiRes = await response.Content.ReadAsStringAsync();
                restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(apiRes);
            }
            return View(restaurants);
        }

        public IActionResult AddRestaurant()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            ViewBag.Conflict = null;
            ViewBag.BadRequest = null;
            return View(new Restaurant()); // ✅ send a fresh Restaurant object to the view
        }


        [HttpPost]
        public async Task<IActionResult> AddRestaurant(Restaurant restaurant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //if (restaurant == null)
            //    return View(new Restaurant());
            //_restaurantService.Add(restaurant);
            //using (var httpClient = new HttpClient())
            //{
                using (var response = await _httpClient.PostAsync("http://localhost:5018/api/restaurantAPI",
                    new StringContent(JsonConvert.SerializeObject(restaurant), Encoding.UTF8,
                    "application/json")))
                {
                    string apiRes = await response.Content.ReadAsStringAsync();
                    //if (!response.IsSuccessStatusCode)// == System.Net.HttpStatusCode.BadRequest)
                    //    ViewBag.BadRequest = "Please ensure that all fields are complete";
                    //else
                        //ViewBag.BadRequest = null;
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        ViewBag.Conflict = "There was a conflict";
                    else
                        ViewBag.Conflict = null;
                    if (ViewBag.Conflict != null)
                        return View(new Restaurant());
                }
            //}
            return RedirectToAction("ManageRestaurants");
        }

        public async Task<IActionResult> EditRestaurant(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            Restaurant restaurant = JsonConvert.DeserializeObject<Restaurant>
                (await(await _httpClient.GetAsync
                ($"http://localhost:5018/api/restaurantAPI/{id}"))
                .Content.ReadAsStringAsync());
            return View(restaurant);
        }

        [HttpPost]
        public async Task<IActionResult> EditRestaurant(Restaurant restaurant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //Restaurant restaurant = JsonConvert.DeserializeObject<Restaurant>
            //    (await (await _httpClient.GetAsync
            //    ($"http://localhost:5018/api/restaurantAPI/{id}"))
            //    .Content.ReadAsStringAsync());
            using (var response = await _httpClient.PutAsync("http://localhost:5018/api/restaurantAPI/" +
                $"{restaurant.Id}/update",
                new StringContent(JsonConvert.SerializeObject(restaurant), Encoding.UTF8, "application/json")))
            {
                var apiRes = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    ViewBag.Error = "Please ensure all fields are complete";
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    ViewBag.Error = $"Restaurant with ID {restaurant.Id} not found";
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    ViewBag.Error = $"A restaurant already exists at that location";
                else
                    ViewBag.Error = null;
                if (!response.IsSuccessStatusCode)
                    return View(restaurant);
            }
            //var restaurant = _restaurantService.Get(id);
            return RedirectToAction("ManageRestaurants");
        }

        //[HttpPost]
        //public IActionResult EditRestaurant(Restaurant restaurant)
        //{
        //    if (!IsAdmin()) return RedirectToAction("Login", "Auth");
        //    _restaurantService.Update(restaurant);
        //    return RedirectToAction("ManageRestaurants");
        //}

        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //_restaurantService.Delete(id);
            using (var response = await _httpClient.DeleteAsync($"http://localhost:5018/api/restaurantAPI/{id}"))
            {
                if (!response.IsSuccessStatusCode)
                    ViewBag.Error = $"Failed to delete restaurant with ID {id}";
                else
                    ViewBag.Error = null;
            }
            return RedirectToAction("ManageRestaurants");
        }

        public async Task<IActionResult> ModerateReviews()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //var reviews = _reviewService.GetAll();
            List<Review> reviews = new List<Review>();
            using (var response = await _httpClient.GetAsync("http://localhost:5018/api/reviewAPI"))
                reviews = JsonConvert.DeserializeObject<List<Review>>
                    (await response.Content.ReadAsStringAsync());
            return View(reviews);
        }

        public async Task<IActionResult> DeleteReview(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            //_reviewService.Delete(id);
            using(var response = await _httpClient.DeleteAsync("http://localhost:5018/api/reviewAPI/" +
                $"{id}"))
            {
                if (!response.IsSuccessStatusCode)
                    ViewBag.Error = $"Failed to delete review with ID {id}";
                else 
                    ViewBag.Error = null;
            }
            return RedirectToAction("ModerateReviews");
        }
    }
}
