using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Validation.Core.Types;

namespace Validation.Infrastructure.Common.Caching;

public class CacheProvider(HybridCache hybridCache) //: ICacheProvider
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockManager = new ConcurrentDictionary<string, SemaphoreSlim>();
    private readonly HybridCache _hybridCache = hybridCache;


    //public async Task<T> GetOrCreate<T>(Func<Task<T>> getData, string itemKey, string tenantID, int cacheForMinutes)
    //{
    //    string cacheKey = CreateTenantKey(itemKey,tenantID);
    //    return await _hybridCache.GetOrCreateAsync<Task<T>>(cacheKey, getData, new HybridCacheEntryOptions { LocalCacheExpiration = TimeSpan.FromMinutes(5) });


    //}

    public async Task<T> GetOrCreate<T>(Func<Task<T>> getData, string itemKey,int cacheForMinutes)
    {

        return await _hybridCache.GetOrCreateAsync<T>(itemKey, async _ => await getData(), new HybridCacheEntryOptions { LocalCacheExpiration = TimeSpan.FromMinutes(cacheForMinutes) });
    }


}
