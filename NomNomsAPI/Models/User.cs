using System.ComponentModel.DataAnnotations;

namespace NomNomsAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // e.g., "Admin" or "User"
    }
}