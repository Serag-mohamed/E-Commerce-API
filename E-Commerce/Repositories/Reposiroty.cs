using E_Commerce.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _entities;

        public Repository(AppDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

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
        
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity == null)
                return false;

            _entities.Remove(entity);
            return true;
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
