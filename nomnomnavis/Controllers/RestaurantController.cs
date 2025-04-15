using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using nomnomnavis.Models;
using System.Numerics;
using System.Text.Json;

namespace nomnomnavis.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5018/api/RestaurantAPI";

        public RestaurantController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var allRestaurants = await _httpClient.GetFromJsonAsync<List<Restaurant>>(_apiBaseUrl);
            return View(allRestaurants);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchTerm, string cuisine, string sort)
        {
            var allRestaurants = await _httpClient.GetFromJsonAsync<List<Restaurant>>(_apiBaseUrl);
            var allReviews = await _httpClient.GetFromJsonAsync<List<Review>>("http://localhost:5018/api/reviewAPI");

            if (!string.IsNullOrEmpty(searchTerm))
                allRestaurants = allRestaurants
                    .Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                             || r.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrEmpty(cuisine))
                allRestaurants = allRestaurants.Where(r => r.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort == "highest-rated")
                {
                    List<Vector2> highestRatedRestaurants = new List<Vector2>();
                    foreach (Restaurant restaurant in allRestaurants)
                    {
                        highestRatedRestaurants.Add(new Vector2(restaurant.Id, (float)(allReviews.Where(r => r.Id == restaurant.Id).Average(rv => rv.Rating))));
                    }
                    highestRatedRestaurants.OrderByDescending(hrr => hrr.Y);
                    List<Restaurant> restaurants = new List<Restaurant>();
                    foreach (Vector2 rank in highestRatedRestaurants)
                    {
                        restaurants.Add(allRestaurants.First(r => r.Id == rank.X));
                    }
                    allRestaurants = restaurants;
                    //allRestaurants = allRestaurants.OrderByDescending(r => r.Reviews.Average(rv => rv.Rating)).ToList();
                }
                else if (sort == "most-reviewed")
                {
                    List<Vector2> mostReviewed = new List<Vector2>();
                    foreach (Restaurant restaurant in allRestaurants)
                        mostReviewed.Add(new Vector2(restaurant.Id, (float)allReviews.Where(r => r.Id == restaurant.Id).Count()));
                    mostReviewed.OrderByDescending(mr => mr.Y);
                    List<Restaurant> restaurants = new List<Restaurant>();
                    foreach(Vector2 rank in mostReviewed)
                        restaurants.Add(allRestaurants.First(r => r.Id == rank.X));
                    allRestaurants = restaurants;
                    //allRestaurants = allRestaurants.OrderByDescending(r => r.Reviews.Count).ToList();
                }
            }

            return View(allRestaurants);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            if (id > 0)
                TempData["RestID"] = id;
            if (TempData["RestID"] != null)
                id = (int)TempData["RestID"];
            Restaurant restaurant = await _httpClient.GetFromJsonAsync<Restaurant>($"{_apiBaseUrl}/{id}");
            //using(var response = await _httpClient.GetAsync($"http://localhost:5018/api/reviewAPI/{id}/reviews"))
            //{
            //    string apiRes = await response.Content.ReadAsStringAsync();
            //    ViewBag.Reviews = JsonConvert.DeserializeObject<List<Review>>(apiRes);
            //}
            ViewBag.Reviews = await _httpClient.GetFromJsonAsync<List<Review>>($"http://localhost:5018/api/reviewAPI/{id}/reviews");
            return View(restaurant);
        }

        public IActionResult AddReview(int restID)
        {
            TempData["RestID"] = restID;
            return RedirectToAction("Add", "Review");
        }

        public IActionResult Favorite(int id)
        {
            List<int> favoriteIds = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();
            if (!favoriteIds.Contains(id))
                favoriteIds.Add(id);
            HttpContext.Session.SetObjectAsJson("Favorites", favoriteIds);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Favorites()
        {
            List<int> favoriteIds = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();
            var allRestaurants = await _httpClient.GetFromJsonAsync<List<Restaurant>>(_apiBaseUrl);
            var favorites = allRestaurants.Where(r => favoriteIds.Contains(r.Id)).ToList();

            return View(favorites);
        }
    }
}

// Helper class for session management
public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object value) =>
        session.SetString(key, JsonConvert.SerializeObject(value));

    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}

