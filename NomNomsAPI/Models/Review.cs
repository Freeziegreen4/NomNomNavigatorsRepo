using System.ComponentModel.DataAnnotations;

namespace NomNomsAPI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
