using E_Commerce.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace E_Commerce.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            HandelSoftDelete(eventData);

            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync
            (
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
            )
        {
            HandelSoftDelete(eventData);
            return new ValueTask<InterceptionResult<int>>(result);
        }
        private static void HandelSoftDelete(DbContextEventData eventData)
        {
            if (eventData.Context is null)
                return;

            foreach (var entry in eventData.Context.ChangeTracker.Entries<ISoftDeleteable>())
            {
                if (entry is not { State: EntityState.Deleted, Entity: ISoftDeleteable entity })
                    continue;

                entry.State = EntityState.Modified;
                entity.Delete();
            }
        }
    }
}


