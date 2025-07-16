using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Concurrent;

namespace Validation.Infrastructure.Common.Caching;

public class CacheProvider(HybridCache hybridCache) //: ICacheProvider
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockManager = new ConcurrentDictionary<string, SemaphoreSlim>();
    private readonly HybridCache _hybridCache = hybridCache;

    public async Task<T> GetOrCreate<T>(Func<Task<T>> getData, string itemKey,int cacheForMinutes)
    {
        return await _hybridCache.GetOrCreateAsync<T>(itemKey, async _ => await getData(), new HybridCacheEntryOptions { LocalCacheExpiration = TimeSpan.FromMinutes(cacheForMinutes) });
    }


}
