using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Query();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task<bool> DeleteAsync(Guid id);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChangesAsync();
    }
}
