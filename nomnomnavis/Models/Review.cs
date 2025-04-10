namespace nomnomnavis.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public int RestaurantId { get; set; }
    }
}
