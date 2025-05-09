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

        public async Task<User> GetUser(Guid id)
        {
            return await _repository.SelectAll().Where(item => item.Id == id).FirstOrDefaultAsync();
        }
    }
}
