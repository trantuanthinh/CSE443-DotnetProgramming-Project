using Project.Models;

namespace Project.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignUp(User user);
        Task<User> SignIn(string email, string password);

        // bool IsValidUser(string username, string password);
    }
}
