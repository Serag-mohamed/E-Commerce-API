using E_Commerce.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Repositories
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private Dictionary<string, object>? _repositories;

        public IRepository<T> Repository<T>() where T : class
        {
            _repositories ??= [];

            var type = typeof(T).Name;

            if (!_repositories.TryGetValue(type, out object? value))
            {
                var repositoryInstance = new Repository<T>(context);
                value = repositoryInstance;
                _repositories.Add(type, value);
            }

            return (IRepository<T>)value!;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
        public void Dispose()
        { 
            context.Dispose();
        }
    }
}
