using Microsoft.EntityFrameworkCore.Storage;

namespace E_Commerce.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}
