﻿namespace nomnomnavis.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Cuisine { get; set; }
        public string Hours { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        // Check Review model for why this was added
    }
}
