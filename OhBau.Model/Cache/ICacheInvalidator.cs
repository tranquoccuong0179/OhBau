using Microsoft.Extensions.Primitives;

public interface ICacheInvalidator<TEntity>
{
void InvalidateEntity(Guid entityId);
    void InvalidateEntityList();
    void SetEntityCache(Guid entityId, object data, TimeSpan? absoluteExpire = null);
    T GetEntityCache<T>(Guid entityId);
}