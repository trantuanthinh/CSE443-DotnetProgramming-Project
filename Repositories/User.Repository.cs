using Project.AppContext;
using Project.Core.Base.Repository;
using Project.Models;

namespace Project.Repositories
{
    public class UserRepository(DataContext context)
        : BaseRepository<User, DataContext>(context) { }
}
