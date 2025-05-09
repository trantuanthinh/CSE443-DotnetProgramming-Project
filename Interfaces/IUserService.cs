using Project.Models;

namespace Project.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(Guid id);
    }
}
