using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using nomnomnavis.Models;



namespace nomnomnavis.Controllers
{
    public class ReviewController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReviewController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5018/api/ReviewAPI/");
        }

        // View all reviews for a restaurant
        public async Task<IActionResult> RestaurantReviews(int restID)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Review>>($"{restID}/reviews");
            return View(response);
        }

        // GET: Add Review form
        public IActionResult Add(int restID)
        {
            ViewBag.RestaurantId = restID;
            return View();
        }

        // POST: Submit review
        [HttpPost]
        public async Task<IActionResult> Add(Review review, int restID)
        {
            // Assign a dummy user or handle logged-in user logic
            review.User = new User { Id = 1 }; // Dummy user
            var response = await _httpClient.PostAsJsonAsync("", review);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("RestaurantReviews", new { restID });

            ModelState.AddModelError(string.Empty, "Error submitting review.");
            return View(review);
        }

        // GET: Edit review
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _httpClient.GetFromJsonAsync<Review>($"{id}");
            return View(review);
        }

        // POST: Edit review
        [HttpPost]
        public async Task<IActionResult> Edit(Review review)
        {
            var response = await _httpClient.PutAsJsonAsync($"{review.Id}/update", review);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("RestaurantReviews", new { restID = review.User.Id }); // Adjust as needed

            ModelState.AddModelError(string.Empty, "Failed to update.");
            return View(review);
        }

        // Delete review
        public async Task<IActionResult> Delete(int id, int restID)
        {
            var response = await _httpClient.DeleteAsync($"{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("RestaurantReviews", new { restID });

            return Content("Error deleting review.");
        }
    }

}
