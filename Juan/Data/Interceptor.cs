using Juan.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Juan.Data;

public class Interceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            eventData.Context.ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Modified).ToList().ForEach(e => e.Entity.UpdatedAt=DateTime.Now);
            eventData.Context.ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Added).ToList().ForEach(e => e.Entity.CreatedAt=DateTime.Now);
        }
        return base.SavingChanges(eventData, result);
    }
}
