using Project.AppContext;
using Project.Core.Base.Repository;
using Project.Models;

namespace Project.Repositories
{
    public class BorrowTransactionRepository(DataContext context)
        : BaseRepository<BorrowTransaction, DataContext>(context) { }
}
