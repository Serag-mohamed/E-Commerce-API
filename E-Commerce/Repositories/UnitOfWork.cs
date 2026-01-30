using E_Commerce.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private Dictionary<string, object>? _repositories;  
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IRepository<T> Repository<T>() where T : class
        {
            _repositories ??= new Dictionary<string, object>();

            var type = typeof(T).Name;

            if (!_repositories.TryGetValue(type, out object? value))
            {
                var repositoryInstance = new Repository<T>(_context);
                value = repositoryInstance;
                _repositories.Add(type, value);
            }

            return (IRepository<T>)value!;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        { 
            _context.Dispose();
        }
    }
}
