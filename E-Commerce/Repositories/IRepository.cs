using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Query();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChangesAsync();
    }
}
