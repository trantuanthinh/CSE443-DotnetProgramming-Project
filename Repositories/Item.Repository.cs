using Project.AppContext;
using Project.Core.Base.Repository;
using Project.Models;

namespace Project.Repositories
{
    public class ItemRepository(DataContext context)
        : BaseRepository<Item, DataContext>(context) { }
}
