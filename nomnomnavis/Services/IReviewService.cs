using nomnomnavis.Models;
using System.Collections.Generic;

namespace nomnomnavis.Services
{
    public interface IReviewService
    {
        List<Review> GetAll();
        void Delete(int id);
    }
}
