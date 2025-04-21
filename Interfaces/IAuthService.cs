using Project.Models;

namespace Project.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignUp(User user);
        Task<User> SignIn(string email, string password);
        string GetGoogleUrl();
        Task<string> ExchangeCodeForTokenAsync(string code, string type);
        Task<string> GetUserInfoAsync(string accessToken);
        Task<User> GoogleAuthenticatedUser(string email);
        Task<bool> CreateGoogleUser(User _item);
    }
}
