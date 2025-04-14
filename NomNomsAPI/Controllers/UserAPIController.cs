using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NomNomsAPI.Models;

namespace NomNomsAPI.Controllers
{
    // Base URL -> http://localhost:5245/api/UserAPI/
    [Route("api/[controller]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        NomNomDBContext nomNomDBAccessor;
        public UserAPIController(
            NomNomDBContext nomNomDBContext) => nomNomDBAccessor = nomNomDBContext;

        // Users API Hooks:
        /*  ✔️ GET -> Single user (
         *      url extension -> http://localhost:5245/api/UserApi/ [userID]
         *  ✔️ GET -> All Users
         *      url extension -> http://localhost:5245/api/UserApi
         *  ✔️ POST -> Create a user
         *      url extension -> http://localhost:5245/api/UserApi
         *  ✔️ PUT -> Update user details
         *      url extension -> http://localhost:5245/api/UserApi/ [userID] /update
         *  ✔️ DELETE -> Delete a user
         *      url extension -> http://localhost:5245/api/UserApi/ [userID]
         */

        /*
         * Uses IEnumerable so you can use any subclass of it (ie. List)
         */
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers() => nomNomDBAccessor.users;

        /* This may change, depends on how we do the
         *  validation logic in the end.
         *  
         * To send data to this hook, use the FormUrlEncodedContent type
         *  and init it like this:
         *  FormUrlEncodedContent content = new FormUrlEncodedContent([
         *      new KeyValuePair<string, string>("uName", "user"),
         *      new KeyValuePair<string, string>("passwd", "pass")
         *  ]);
         */
        [HttpGet("{userID}")]
        public ActionResult<User> GetAUser(int userID)//[FromForm] string uName, [FromForm] string passwd)
        {
            if (userID <= 0)//uName == null || uName.Equals("")
                //|| passwd == null || passwd.Equals(""))
                // To use this, use an if case checking
                //  [responseVarName].StatusCode == System.Net.HttpStatusCode.OK
                return BadRequest();
            User currentUser = nomNomDBAccessor.users.FirstOrDefault
                (u => u.Id == userID);
                //(u => u.Username.Equals(uName) && u.Password.Equals(passwd));
            if(currentUser == null)
                return NotFound("User could not be found. Ensure you have the correct ID");

            return Ok(currentUser);
        }

        [HttpPost]
        public ActionResult<User> AddUser(User user)
        {
            // Validation
            if(!CheckUserValues(user))
                return BadRequest("Ensure all fields are not blank");
            else if (nomNomDBAccessor.users.FirstOrDefault
                (u => u.Username == user.Username) != null)
                return Conflict("A user with that username already exists");
            User newUser = new User()
            {
                Username = user.Username,
                Password = user.Password,
                Role = user.Role
            };

            if (newUser == null)
                return BadRequest("An error occurred when" +
                    "creating the user, please try again!");

            nomNomDBAccessor.users.Add(newUser);
            nomNomDBAccessor.SaveChanges();
            return Ok(newUser);
        }

        [Route("{userID}/update")]
        [HttpPut]
        public ActionResult<User> UpdateUser(int userID, [FromBody] User userInfo)
        {
            if (userID <= 0)
                return BadRequest("ID must be greater than zero!");
            User userToUpdate = nomNomDBAccessor.users.FirstOrDefault(u => u.Id == userID);
            if (userToUpdate == null)
                return BadRequest($"User with ID {userID} does not exist!");
            else if (!CheckUserValues(userInfo))
                return BadRequest("Invalid values. Cannot update with blank values");
            else if (nomNomDBAccessor.users.FirstOrDefault(
                u => u.Id != userID && u.Username.Equals(userInfo.Username)) != null)
                return BadRequest("A user with username already exists");

            userToUpdate.Username = userInfo.Username;
            userToUpdate.Password = userInfo.Password;
            userToUpdate.Role = userInfo.Role;
            nomNomDBAccessor.users.Update(userToUpdate);
            nomNomDBAccessor.SaveChanges();

            User checkUserForNewValues = nomNomDBAccessor.users.First(u => u.Id == userID);
            if (!checkUserForNewValues.Username.Equals(userToUpdate.Username)
                || !checkUserForNewValues.Password.Equals(userToUpdate.Password)
                || !checkUserForNewValues.Role.Equals(userToUpdate.Role))
                return BadRequest("Failed to update user");

            return Ok(userToUpdate);
        }

        [HttpDelete("{userID}")]
        public ActionResult DeleteUser(int userID)
        {
            if (userID <= 0)
                return BadRequest("Id must be greater than zero!");
            User userToDelete = nomNomDBAccessor.users.FirstOrDefault(u => u.Id == userID);
            if (userToDelete == null)
                return BadRequest($"User with ID {userID} does not exist!");

            nomNomDBAccessor.users.Remove(userToDelete);
            nomNomDBAccessor.SaveChanges();

            if (nomNomDBAccessor.users.FirstOrDefault(u => u.Id == userID) != null)
                return BadRequest("Failed to delete user");

            return Ok($"Successfully deleted user: {userToDelete.Username}");
        }

        private bool CheckUserValues(User user)
        {

            return user.Username != null && !user.Username.Equals("")
                && user.Password != null && !user.Password.Equals("")
                && user.Role != null && !user.Role.Equals("");
        }
    }
}
