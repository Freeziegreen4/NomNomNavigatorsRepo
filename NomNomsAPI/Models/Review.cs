using System.ComponentModel.DataAnnotations;

namespace NomNomsAPI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        [Required(ErrorMessage = "Review content cannot be blank!")]
        public string Content { get; set; }
        public int Rating { get; set; }
        public int RestaurantID { get; set; }
    }
}
