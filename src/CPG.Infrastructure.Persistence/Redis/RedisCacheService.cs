using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Text.Json;

namespace CPG.Infrastructure.Persistence.Redis
{
    public class RedisCacheService: IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration configuration;

        public RedisCacheService(IDistributedCache cache,IConfiguration configuration)
        {
            _cache = cache;
            this.configuration = configuration;
        }

        public T GetData<T>(string key)
        {
            var jsonData = _cache.GetString(key);
            if (jsonData == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(jsonData);
        }


        public void SetData<T>(string key, T value)
        {
           
            var cacheDuration = TimeSpan.FromSeconds(int.Parse(configuration["Redis:CacheDurationSeconds"]));
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };
            var jsonData = JsonSerializer.Serialize(value);
            _cache.SetString(key, jsonData, options);

           
        }
        public void RemoveKey(string key)
        {
            _cache.Remove(key);
        }
      
    }
}
