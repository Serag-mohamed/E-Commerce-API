using E_Commerce.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories
{
    public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _entities = context.Set<T>();

        public IQueryable<T> Query()
        {
            return _entities.AsQueryable(); 
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _entities.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }
        
        public void Update(T entity)
        {
            _entities.Update(entity);
        }
        
        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            context.RemoveRange(entities);
        }
    }

}
