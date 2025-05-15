using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;
using Project.Utils;

namespace Project.Services
{
    public class UserService(UserRepository repository) : IUserService
    {
        private readonly UserRepository _repository = repository;

        public async Task<ICollection<User>> GetLecturers()
        {
            return await _repository
                .SelectAll()
                .Where(item => item.Role == UserType.Lecturer)
                .ToListAsync();
        }

        public async Task<ICollection<User>> GetManagers()
        {
            return await _repository
                .SelectAll()
                .Where(item => item.Role == UserType.Manager)
                .ToListAsync();
        }

        public async Task<User> GetUser(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> EditUser(User item)
        {
            var existingItem = await GetUser(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.Name = item.Name;
            existingItem.Username = item.Username;
            if (!string.IsNullOrWhiteSpace(item.Password))
            {
                existingItem.Password = BCrypt.Net.BCrypt.HashPassword(item.Password);
            }
            existingItem.PhoneNumber = item.PhoneNumber;

            _repository.Update(existingItem);
            return await _repository.SaveAsync();
        }
    }
}
