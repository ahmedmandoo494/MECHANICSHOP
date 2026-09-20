using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MechanicShop.Infrastructure.Data.Interceptors;

public class AuditableentityInterceptor(IUser user,TimeProvider timeProvider) : SaveChangesInterceptor
{
    private readonly IUser _user = user;
    private readonly TimeProvider _timeProvider = timeProvider;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if(context is null)
        {
            return;
        }
        foreach(var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            var utcNow = _timeProvider.GetUtcNow();
            if(entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.CreatedBy = _user.Id;
                }
                entry.Entity.LastModifiedAtUtc = utcNow;
                entry.Entity.LastModifiedBy = _user.Id;
            }
            foreach (var ownedEntry in entry.References)
            {
                if(ownedEntry.TargetEntry is {Entity : AuditableEntity ownedEntity} && ownedEntry.TargetEntry.State is EntityState.Added or EntityState.Modified )
                {
                    if(ownedEntry.TargetEntry.State == EntityState.Added)
                    {
                        ownedEntity.CreatedAtUtc = utcNow;
                        ownedEntity.CreatedBy = _user.Id;
                    }
                    ownedEntity.LastModifiedAtUtc = utcNow;
                    ownedEntity.LastModifiedBy = _user.Id;
                }
            }
        }
    }

}

public static class Extentions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) => 
        entry.References.Any(r =>
        r.TargetEntry?.Metadata.IsOwned() == true &&
        (r.TargetEntry?.State == EntityState.Added ||r.TargetEntry?.State == EntityState.Modified));
}
