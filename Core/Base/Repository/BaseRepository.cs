using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project.Core.Base.Enity;

namespace Project.Core.Base.Repository
{
    // DONE
    public class BaseRepository<T, DbContextType> : IBaseRepository<T>
        where T : BaseEntity<Guid>
        where DbContextType : DbContext
    {
        protected readonly DbContextType _dbContext;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContextType context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> SelectAll()
        {
            return _dbSet;
        }

        public T SelectById(Guid id)
        {
            return _dbSet.Find(id);
        }

        // public DbSet<T> GetDbSet()
        // {
        //     return _dbSet;
        // }

        public T Add(T obj)
        {
            var createdAtProperty = obj.GetType().GetProperty("Created_At");
            var modifiedAtProperty = obj.GetType().GetProperty("Modified_At");
            var timestampAtProperty = obj.GetType().GetProperty("Timestamp");
            var isActiveAtProperty = obj.GetType().GetProperty("IsActive");
            if (createdAtProperty != null && createdAtProperty.CanWrite)
            {
                createdAtProperty.SetValue(obj, DateTime.UtcNow);
            }
            if (modifiedAtProperty != null && modifiedAtProperty.CanWrite)
            {
                modifiedAtProperty.SetValue(obj, DateTime.UtcNow);
            }
            if (timestampAtProperty != null && timestampAtProperty.CanWrite)
            {
                timestampAtProperty.SetValue(obj, DateTime.UtcNow);
            }
            if (isActiveAtProperty != null && isActiveAtProperty.CanWrite)
            {
                isActiveAtProperty.SetValue(obj, true);
            }
            return _dbSet.Add(obj).Entity;
        }

        public async Task AddRangeAsync(IList<T> objs)
        {
            foreach (var item in objs)
            {
                var createdAtProperty = item.GetType().GetProperty("Created_At");
                var modifiedAtProperty = item.GetType().GetProperty("Modified_At");
                var timestampAtProperty = item.GetType().GetProperty("Timestamp");
                if (createdAtProperty != null && createdAtProperty.CanWrite)
                {
                    createdAtProperty.SetValue(item, DateTime.UtcNow);
                }
                if (modifiedAtProperty != null && modifiedAtProperty.CanWrite)
                {
                    modifiedAtProperty.SetValue(item, DateTime.UtcNow);
                }
                if (timestampAtProperty != null && timestampAtProperty.CanWrite)
                {
                    timestampAtProperty.SetValue(item, DateTime.UtcNow);
                }
            }
            await _dbSet.AddRangeAsync(objs);
        }

        public T Update(T obj)
        {
            var modifiedAtProperty = obj.GetType().GetProperty("Modified_At");
            if (modifiedAtProperty != null && modifiedAtProperty.CanWrite)
            {
                modifiedAtProperty.SetValue(obj, DateTime.UtcNow);
            }
            return _dbSet.Update(obj).Entity;
        }

        public async Task UpdateRangeAsync(IList<T> objs)
        {
            foreach (var item in objs)
            {
                var modifiedAtProperty = item.GetType().GetProperty("Modified_At");
                if (modifiedAtProperty != null && modifiedAtProperty.CanWrite)
                {
                    modifiedAtProperty.SetValue(item, DateTime.UtcNow);
                }
            }
            _dbSet.UpdateRange(objs);
            await _dbContext.SaveChangesAsync();
        }

        public T Delete(Guid id)
        {
            var obj = _dbSet.Where(x => x.Id == id).FirstOrDefault();
            return _dbSet.Remove(obj).Entity;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _dbContext.Database.BeginTransactionAsync();
        }

        public bool Save()
        {
            int saved = _dbContext.SaveChanges();
            return saved > 0;
        }

        public async Task<bool> SaveAsync()
        {
            int saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}
