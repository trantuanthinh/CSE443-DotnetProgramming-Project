using Project.Models;

namespace Project.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<User>> GetLecturers();
        Task<User> GetUser(Guid id);
        Task<bool> EditUser(User item);
    }
}
