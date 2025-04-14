using Project.Core.Base.Enity;

namespace Project.Core.Base.Repository
{
    // DONE
    public interface IBaseRepository<T>
        where T : BaseEntity<Guid>
    {
        public IQueryable<T> SelectAll();
        public T SelectById(Guid id);
        public T Add(T obj);
        public T Update(T obj);
        public T Delete(Guid id);
        public bool Save();
        public Task<bool> SaveAsync();
    }
}
