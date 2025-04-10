using nomnomnavis.Data;
using nomnomnavis.Models;
using System.Collections.Generic;
using System.Linq;

namespace nomnomnavis.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public List<Review> GetAll() => _context.Reviews.ToList();

        public void Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }
    }
}
