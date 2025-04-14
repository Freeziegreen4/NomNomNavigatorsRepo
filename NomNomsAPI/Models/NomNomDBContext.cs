using Microsoft.EntityFrameworkCore;

namespace NomNomsAPI.Models
{
    public class NomNomDBContext : DbContext
    {
        public NomNomDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> users => Set<User>();
        public DbSet<Restaurant> restaurants => Set<Restaurant>();
        public DbSet<Review> reviews => Set<Review>();
    }
}
