using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

public abstract class BaseCacheInvalidator<TEntity> : ICacheInvalidator<TEntity>
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultExpiration;
    private CancellationTokenSource _listCacheTokenSource = new CancellationTokenSource();

    protected BaseCacheInvalidator(IMemoryCache cache, TimeSpan? defaultExpiration = null)
    {
        _cache = cache;
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
    }

    protected abstract string GetEntityCacheKey(Guid entityId);

    public void InvalidateEntity(Guid entityId)
    {
        _cache.Remove(GetEntityCacheKey(entityId));
    }

    public void InvalidateEntityList()
    {
        if (!_listCacheTokenSource.Token.IsCancellationRequested)
        {
            _listCacheTokenSource.Cancel();
        }
        _listCacheTokenSource = new CancellationTokenSource();
    }

    public CancellationChangeToken GetListCacheToken() => new CancellationChangeToken(_listCacheTokenSource.Token);

    public void SetEntityCache(Guid entityId, object data, TimeSpan? absoluteExpire = null)
    {
        var key = GetEntityCacheKey(entityId);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpire ?? _defaultExpiration
        };
        _cache.Set(key, data, options);
    }

    public bool TryGetEntityCache<T>(Guid entityId, out T value)
    {
        var key = GetEntityCacheKey(entityId);
        if (_cache.TryGetValue(key, out value))
        {
            return true;
        }
        value = default;
        return false;
    }

    public T GetEntityCache<T>(Guid entityId)
    {
        if (TryGetEntityCache(entityId, out T value))
        {
            return value;
        }
        return default;
    }

    protected virtual string GetCacheKey(object parameters)
    {
        var key = $"{typeof(TEntity).Name}_List_";
        if (parameters == null)
            return key + "Default";

        if (parameters is ListParameters<TEntity> listParams)
        {
            key += $"Page_{listParams.PageNumber}_Size_{listParams.PageSize}";
            foreach (var filter in listParams.Filters)
            {
                var filterValue = filter.Value?.ToString()?.Replace(" ", "_") ?? "Null";
                key += $"_{filter.Key}_{filterValue}";
            }
        }
        else
        {
            var paramString = string.Join("_", parameters.GetType().GetProperties()
                .Select(p => $"{p.Name}_{p.GetValue(parameters)}"));
            key += paramString;
        }

        return key;
    }
}