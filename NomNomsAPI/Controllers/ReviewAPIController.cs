using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NomNomsAPI.Models;

namespace NomNomsAPI.Controllers
{
    // Base URL -> http://localhost:5018/api/ReviewAPI/
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAPIController : ControllerBase
    {
        NomNomDBContext nomNomDBAccessor;
        public ReviewAPIController(
            NomNomDBContext nomNomDBContext) => nomNomDBAccessor = nomNomDBContext;

        // Review API Hooks
        /*  GET -> Single review (
         *      url extension -> http://localhost:5018/api/ReviewAPI/ [reviewID]
         *  GET -> All reviews
         *      url extension -> http://localhost:5018/api/ReviewAPI/
         *  GET -> All reviews for a specific restaurant
         *      url extension -> http://localhost:5018/api/ReviewAPI/ [restID] /reviews/
         *      
         *      ***************************************************************************
         *      
         *  POST -> Create a review
         *      url extension -> http://localhost:5018/api/ReviewAPI/
         *      
         *  PUT -> Update review
         *      url extension -> http://localhost:5018/api/ReviewAPI/ [reviewID] /update
         *      
         *  DELETE -> Delete a review
         *      url extension -> http://localhost:5018/api/ReviewAPI/ [reviewID]
         *      
         *      ***************************************************************************
         */

        [HttpGet("{reviewID}")]
        public ActionResult<Review> GetAReview(int reviewID)
        {
            if (reviewID <= 0)
                return BadRequest("ID must be greater than zero!");
            Review currentReview = nomNomDBAccessor.reviews.FirstOrDefault
                (r => r.Id == reviewID);
            if (currentReview == null)
                return NotFound("Review could not be found. Ensure you have the correct ID");

            return Ok(currentReview);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Review>> GetReviews() => nomNomDBAccessor.reviews;

        [Route("{restID}/reviews")]
        [HttpGet]
        public ActionResult<IEnumerable<Review>> GetReviewsByRestaurantID(int restID)
        {
            if (restID <= 0)
                return BadRequest("ID must be greater than zero!");
            Restaurant restaurant = nomNomDBAccessor
                .restaurants.FirstOrDefault(r => r.Id == restID);
            if (restaurant == null)
                return BadRequest("There is no restaurant for the given ID");
            else if (restaurant.Reviews.Count() == 0)
                return BadRequest($"There are no reviews for {restaurant.Name} at {restaurant.Address}");

            return Ok(restaurant.Reviews);
        }

        [HttpPost]
        public ActionResult<Review> AddReview([FromBody] Review review)
        {
            if (!CheckReviewValues(review))
                return BadRequest("Ensure that all fields are not blank");
            else if (nomNomDBAccessor.reviews.FirstOrDefault(
                r => r.Id == review.Id) != null)
                return Conflict("An error occurred when adding the review. " +
                    "Please try again!");
            //else if (nomNomDBAccessor.reviews.First(r => r. == review.Address) != null)
            //    return Conflict($"There is already a review at that address ({review.Address}). " +
            //        $"Please enter an address that has not already been taken.");
            Review newReview = new Review()
            {
                Content = review.Content,
                User = review.User,
                Rating = review.Rating
            };

            nomNomDBAccessor.reviews.Add(newReview);
            nomNomDBAccessor.SaveChanges();
            if (nomNomDBAccessor.reviews.FirstOrDefault(r => r.Id == review.Id) == null)
                return BadRequest("An error occurred when adding the review. " +
                    "Please try again!");
            return Ok(newReview);
        }

        [Route("{reviewID}/update")]
        [HttpPut]
        public ActionResult<Review> UpdateReview(int reviewID, [FromBody] Review reviewInfo)
        {
            if (reviewID <= 0)
                return BadRequest("ID must be greater than zero!");
            Review reviewToUpdate = nomNomDBAccessor.reviews.FirstOrDefault(u => u.Id == reviewID);
            if (reviewToUpdate == null)
                return BadRequest($"Review with ID {reviewID} does not exist!");
            else if (!CheckReviewValues(reviewInfo))
                return BadRequest("Invalid values. Cannot update with blank values");

            reviewToUpdate.Content = reviewInfo.Content;
            reviewToUpdate.Rating = reviewInfo.Rating;
            nomNomDBAccessor.reviews.Update(reviewToUpdate);
            nomNomDBAccessor.SaveChanges();

            Review checkReviewForNewValues = nomNomDBAccessor.reviews.First(u => u.Id == reviewID);
            if (!checkReviewForNewValues.Content.Equals(reviewToUpdate.Content)
                || checkReviewForNewValues.Rating != reviewToUpdate.Rating)
                return BadRequest("Failed to update review");

            return Ok(reviewToUpdate);
        }

        [HttpDelete("{reviewID}")]
        public ActionResult<Review> DeleteReview(int reviewID)
        {
            if (reviewID <= 0)
                return BadRequest("Id must be greater than zero!");
            Review reviewToDelete = nomNomDBAccessor.reviews.FirstOrDefault(u => u.Id == reviewID);
            if (reviewToDelete == null)
                return BadRequest($"review with ID {reviewID} does not exist!");

            nomNomDBAccessor.reviews.Remove(reviewToDelete);
            nomNomDBAccessor.SaveChanges();

            if (nomNomDBAccessor.reviews.FirstOrDefault(u => u.Id == reviewID) != null)
                return BadRequest("Failed to delete review");

            return Ok($"Successfully deleted review: {reviewToDelete.Id}\n{reviewToDelete.Content}");
        }

        private bool CheckReviewValues(Review review) => 
            review.Content != null && !review.Content.Equals("")
            && review.User != null 
            && nomNomDBAccessor.users.FirstOrDefault(u => u.Id == review.User.Id) != null
            && review.Rating >= 0;
    }
}
