using Microsoft.Extensions.Caching.Memory;

public class GenericCacheInvalidator<TEntity> : BaseCacheInvalidator<TEntity>
{
    public GenericCacheInvalidator(IMemoryCache cache) : base(cache)
    {
    }

    protected override string GetEntityCacheKey(Guid entityId)
    {
        return $"{typeof(TEntity).Name}_{entityId}";
    }

    public string GetCacheKeyForList(object parameters)
    {
        return GetCacheKey(parameters);
    }

    public void AddToListCacheKeys(string cacheKey)
    {
        base.AddToListCacheKeys(cacheKey);
    }
}

public class ListParameters<TEntity>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

    public ListParameters(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public void AddFilter<T>(string key, T value)
    {
        if (value != null)
        {
            Filters[key] = value;
        }
    }
}