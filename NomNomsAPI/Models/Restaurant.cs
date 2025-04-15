using System.ComponentModel.DataAnnotations;

namespace NomNomsAPI.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a name!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter an address!")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter a type of cuisine!")]
        public string Cuisine { get; set; }
        [Required(ErrorMessage = "Please enter operational hours!")]
        public string Hours { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
