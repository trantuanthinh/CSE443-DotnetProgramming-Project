using Microsoft.EntityFrameworkCore;
using Project.AppContext;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class AuthService(UserRepository repository) : IAuthService
    {
        private readonly UserRepository _repository = repository;

        public async Task<bool> SignUp(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _repository.Add(user);
            return await _repository.SaveAsync();
        }

        public async Task<User> SignIn(string email, string password)
        {
            var user = await _repository
                .SelectAll()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }
    }
}
