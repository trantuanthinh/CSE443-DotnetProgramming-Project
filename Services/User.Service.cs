using Project.Interfaces;
using Project.Repositories;

namespace Project.Services
{
    public class UserService(UserRepository repository) : IUserService
    {
        private readonly UserRepository _repository = repository;
    }
}
