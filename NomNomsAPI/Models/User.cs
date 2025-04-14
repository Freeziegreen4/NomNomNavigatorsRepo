using System.ComponentModel.DataAnnotations;

namespace NomNomsAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a username!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter a password!")]
        public string Password { get; set; }
        public string Role { get; set; } // e.g., "Admin" or "User"
    }
}