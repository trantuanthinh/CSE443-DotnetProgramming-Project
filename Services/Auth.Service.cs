using Microsoft.EntityFrameworkCore;
using Project.AppContext;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository _repository;

        public AuthService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> SignUp(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _repository.Add(user);
            await _repository.SaveAsync();

            return user;
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
