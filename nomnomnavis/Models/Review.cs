namespace nomnomnavis.Models
{
    public class Review
    {
        public int Id { get; set; }
        //public string Username { get; set; }
        // Changing the username property to a user object
        // for a similar reason to the removal of restaurantID
        public User User { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        
        //public int RestaurantId { get; set; }

        /* Removed the restaurantID property,
         *  shifted it over as an object in the restaurant model.
         *  David taught me that by using an object like that,
         *  the database implicitly converts it to a FK PK pairing.
         *  Basically C#/ASP.NET/EF has a built in version of what
         *  we were doing
         *  
         * If this breaks what we've done, then I'll be more than
         *  happy to go back to what it was before since I'm more
         *  familiar with that anyway
         */
    }
}
