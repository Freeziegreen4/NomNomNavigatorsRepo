using Microsoft.AspNetCore.Mvc;
using nomnomnavis.Models;
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

        public async Task<IActionResult> Index(string searchTerm, string cuisine, string sort)
        {
            var allRestaurants = await _httpClient.GetFromJsonAsync<List<Restaurant>>(_apiBaseUrl);

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
                    allRestaurants = allRestaurants.OrderByDescending(r => r.Reviews.Average(rv => rv.Rating)).ToList();
                else if (sort == "most-reviewed")
                    allRestaurants = allRestaurants.OrderByDescending(r => r.Reviews.Count).ToList();
            }

            return View(allRestaurants);
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _httpClient.GetFromJsonAsync<Restaurant>($"{_apiBaseUrl}/{id}");
            return View(restaurant);
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
        session.SetString(key, JsonSerializer.Serialize(value));

    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}

