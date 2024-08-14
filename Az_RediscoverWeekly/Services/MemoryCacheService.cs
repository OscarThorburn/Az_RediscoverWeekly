using Microsoft.Extensions.Caching.Memory;
using System;

namespace Az_RediscoverWeekly.Services
{
    /// <summary>
    /// Handles the caching of data in memory. Is used to reduce the number of request to KeyVault.
    /// </summary>
    public class MemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly DataProtectorService _dataProtectorService;

        public MemoryCacheService(IMemoryCache memoryCache, DataProtectorService dataProtectorService)
        {
            _dataProtectorService = dataProtectorService;
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// Try to get the token from the cache and decrypt it.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryGetToken(out string token)
        {
            var result = _memoryCache.TryGetValue("SpotifyAccessToken", out token);
            if (result)
                token = _dataProtectorService.Unprotect(token!);
            return result;
        }
        /// <summary>
        /// Set the token encrypted in the cache lasting the duration of the token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="duration"></param>
        public void SetToken(string token, long duration)
        {
            _memoryCache.Set("SpotifyAccessToken", _dataProtectorService.Protect(token), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(duration - 5)
            });
        }
    }
}

