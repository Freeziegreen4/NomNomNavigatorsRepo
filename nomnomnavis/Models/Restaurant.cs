using System.ComponentModel.DataAnnotations;

namespace nomnomnavis.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a name!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter an address!")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter a type of cuisine!")]
        public string Cuisine { get; set; }
        [Required(ErrorMessage = "Please enter operational hours!")]
        public string Hours { get; set; }
        //public List<Review> Reviews { get; set; } = new List<Review>();
        // Check Review model for why this was added
    }
}
