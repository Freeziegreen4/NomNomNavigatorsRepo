﻿using Microsoft.AspNetCore.Mvc;
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
            List<Review> response = await _httpClient.GetFromJsonAsync<List<Review>>($"{restID}/reviews");
            return View(response);
        }

        // GET: Add Review form
        //[HttpGet("{restID}")]
        public IActionResult Add(int restID)
        {
            if (TempData["RestID"] != null)
                ViewBag.RestaurantId = TempData["RestID"];
            else
                ViewBag.RestaurantId = restID;
            return View(new Review());
        }

        // POST: Submit review
        [HttpPost]
        public async Task<IActionResult> Add(Review review, int restID)
        {
            // Assign a dummy user or handle logged-in user logic
            User currentUser = (await _httpClient.GetFromJsonAsync<List<User>>("http://localhost:5018/api/userAPI"))
                .FirstOrDefault(u => u.Username.Equals(HttpContext.Session.GetString("username")));
            review.Username = currentUser.Username; // Dummy user // NOTE (CAM): gonna make this
                                       // dummy user contain the id of the active user
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5018/api/reviewAPI/{restID}", review);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("RestaurantReviews", restID);

            ModelState.AddModelError(string.Empty, "Error submitting review.");
            TempData["RestID"] = restID;
            return Redirect($"http://localhost:5245/restaurant/details/{restID}");
        }

        // GET: Edit review
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _httpClient.GetFromJsonAsync<Review>($"{id}");
            return View(review);
        }

        // POST: Edit review
        [HttpPost]
        public async Task<IActionResult> Edit(int restID, Review review)
        {
            var response = await _httpClient.PutAsJsonAsync($"{review.Id}/update", review);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("RestaurantReviews", restID); // Adjust as needed

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
