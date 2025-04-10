using nomnomnavis.Models;

namespace nomnomnavis.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        void Register(User user);
    }
}
