using nomnomnavis.Data;
using nomnomnavis.Models;
using System.Collections.Generic;
using System.Linq;

namespace nomnomnavis.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly AppDbContext _context;

        public RestaurantService(AppDbContext context)
        {
            _context = context;
        }

        public List<Restaurant> GetAll() => _context.Restaurants.ToList();
        public Restaurant Get(int id) => _context.Restaurants.Find(id);
        public void Add(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
        }

        public void Update(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var res = _context.Restaurants.Find(id);
            if (res != null)
            {
                _context.Restaurants.Remove(res);
                _context.SaveChanges();
            }
        }
    }
}
