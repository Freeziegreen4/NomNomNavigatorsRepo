using nomnomnavis.Models;
using System.Collections.Generic;

namespace nomnomnavis.Services
{
    public interface IRestaurantService
    {
        List<Restaurant> GetAll();
        Restaurant Get(int id);
        void Add(Restaurant restaurant);
        void Update(Restaurant restaurant);
        void Delete(int id);
    }
}
