using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Utils
{
    public static class CacheKey
    {
        public static string GetEntityCacheKey<TEntity>(Guid entityId)
        {
            return $"{typeof(TEntity).Name}_{entityId}";
        }

        public static string GetEntityListCacheKey<TEntity>(int pageNumber, int pageSize, string? filter)
        {
            return $"{typeof(TEntity).Name}_List_{pageNumber}_{pageSize}_{(filter ?? "All")}";
        }

    }
}
