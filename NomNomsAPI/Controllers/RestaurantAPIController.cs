using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NomNomsAPI.Models;

namespace NomNomsAPI.Controllers
{
    // Base URL -> http://localhost:5245/api/RestaurantAPI/
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantAPIController : ControllerBase
    {
        NomNomDBContext nomNomDBAccessor;
        public RestaurantAPIController(NomNomDBContext nomNomDBContext) => nomNomDBAccessor = nomNomDBContext;

        // Restaurant API Hooks
        /*  ✔️ GET -> Single restaurant (
         *      url extension -> http://localhost:5245/api/RestaurantAPI/ [restID]
         *  ✔️ GET -> All restaurants
         *      url extension -> http://localhost:5245/api/RestaurantAPI/
         *  ✔️ POST -> Create a restaurant
         *      url extension -> http://localhost:5245/api/RestaurantAPI/
         *  ✔️ PUT -> Update restaurant details
         *      url extension -> http://localhost:5245/api/RestaurantAPI/ [restID] /update
         *  ✔️ DELETE -> Delete a restaurant
         *      url extension -> http://localhost:5245/api/RestaurantAPI/ [restID]
         */

        /*
         * Uses IEnumerable so you can use any subclass of it (ie. List)
         */
        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> GetRestaurants() => nomNomDBAccessor.restaurants;

        [HttpGet("{restID}")]
        public ActionResult<Restaurant> GetARestaurant(int restID)
        {
            if (restID <= 0)
                return BadRequest("ID must be greater than zero!");
            Restaurant currentRest = nomNomDBAccessor.restaurants.FirstOrDefault
                (r => r.Id == restID);
            if (currentRest == null)
                return NotFound("Restaurant could not be found. Ensure you have the correct ID");

            return Ok(currentRest);
        }

        [HttpPost]
        public ActionResult<Restaurant> AddRestaurant([FromBody] Restaurant restaurant)
        {
            if (!CheckRestaurantValues(restaurant))
                return BadRequest("Ensure that all fields are not blank");
            else if (nomNomDBAccessor.restaurants.FirstOrDefault(
                r => r.Id == restaurant.Id) != null)
                return Conflict("An error occurred when adding the restaurant. " +
                    "Please try again!");
            else if (nomNomDBAccessor.restaurants.First(r => r.Address == restaurant.Address) != null)
                return Conflict($"There is already a restaurant at that address ({restaurant.Address}). " +
                    $"Please enter an address that has not already been taken.");
            Restaurant newRestaurant = new Restaurant()
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Hours = restaurant.Hours,
                Cuisine = restaurant.Cuisine
            };

            nomNomDBAccessor.restaurants.Add(newRestaurant);
            nomNomDBAccessor.SaveChanges();
            if (nomNomDBAccessor.restaurants.FirstOrDefault(r => r.Id == restaurant.Id) == null)
                return BadRequest("An error occurred when adding the restaurant. " +
                    "Please try again!");
            return Ok(newRestaurant);
        }

        [Route("{restID}/update")]
        [HttpPut]
        public ActionResult<Restaurant> UpdateRestaurant(int restID, [FromBody] Restaurant restaurantInfo)
        {
            if (restID <= 0)
                return BadRequest("ID must be greater than zero!");
            Restaurant restaurantToUpdate = nomNomDBAccessor.restaurants
                .FirstOrDefault(r => r.Id == restID);
            if (restaurantToUpdate == null)
                return BadRequest($"Restaurant with ID {restID} does not exist");
            else if (!CheckRestaurantValues(restaurantInfo))
                return BadRequest("Invalid values. Cannot update with blank values");
            else if (nomNomDBAccessor.restaurants.FirstOrDefault(r => r.Address == restaurantInfo.Address) != null)
                return BadRequest("There is already a restaurant at that address. " +
                    "Please ensure the address is not taken already!");

            restaurantToUpdate.Name = restaurantInfo.Name;
            restaurantToUpdate.Address = restaurantInfo.Address;
            restaurantToUpdate.Hours = restaurantInfo.Hours;
            restaurantToUpdate.Cuisine = restaurantInfo.Cuisine;

            nomNomDBAccessor.restaurants.Update(restaurantToUpdate);
            nomNomDBAccessor.SaveChanges();
            Restaurant checkForNewValues = nomNomDBAccessor.restaurants.First(r => r.Id == restID);
            if (!checkForNewValues.Name.Equals(restaurantToUpdate)
                || !checkForNewValues.Address.Equals(restaurantToUpdate.Address)
                || !checkForNewValues.Hours.Equals(restaurantToUpdate.Hours)
                || !checkForNewValues.Cuisine.Equals(restaurantToUpdate.Cuisine))
                return Conflict($"Failed to update {checkForNewValues.Name}(s) at " +
                    $"{checkForNewValues.Address} values");

            return Ok(restaurantToUpdate);
        }

        [HttpDelete("{restID}")]
        public ActionResult DeleteRestaurant(int restID)
        {
            if (restID <= 0)
                return BadRequest("ID must be greater than zero!");
            Restaurant restaurantToDelete = nomNomDBAccessor.restaurants
                .FirstOrDefault(r => r.Id == restID);
            if (restaurantToDelete == null)
                return BadRequest($"Restaurant with ID {restID} does not exist");

            nomNomDBAccessor.restaurants.Remove(restaurantToDelete);
            nomNomDBAccessor.SaveChanges();

            if (nomNomDBAccessor.restaurants.FirstOrDefault(r => r.Id == restID) != null)
                return BadRequest("Failed to delete restaurant");
            return Ok($"Successfully deleted restaurant: " +
                $"{restaurantToDelete.Name} at {restaurantToDelete.Address}");
        }

        private bool CheckRestaurantValues(Restaurant rest)
        {

            return rest.Name != null && !rest.Name.Equals("")
                && rest.Address != null && !rest.Address.Equals("")
                && rest.Hours != null && !rest.Hours.Equals("")
                && rest.Cuisine != null && !rest.Cuisine.Equals("");
        }
    }
}
