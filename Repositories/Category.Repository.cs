using Project.AppContext;
using Project.Core.Base.Repository;
using Project.Models;

namespace Project.Repositories
{
    public class CategoryRepository(DataContext context)
        : BaseRepository<Category, DataContext>(context) { }
}
