using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SquishIt.Framework.Caches;

namespace SquishIt.Framework
{
    public abstract class ContentCache : IContentCache
    {
        private readonly List<string> CacheKeys = new List<string>();
        protected abstract string KeyPrefix { get; }

        public string GetContent(string name)
        {
            return (string)HttpRuntime.Cache[KeyPrefix + name];
        }

        public void ClearTestingCache()
        {
            foreach (var key in CacheKeys)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        public bool ContainsKey(string key)
        {
            return HttpRuntime.Cache[BuildCacheKey(key)] != null;
        }

        public bool TryGetValue(string key, out string content)
        {
            content = (string)HttpRuntime.Cache[BuildCacheKey(key)];
            return content != null;
        }

        public void Add(string key, string content, List<string> files, bool debuggingEnabled)
        {
            var cacheKey = BuildCacheKey(key);
            CacheKeys.Add(cacheKey);

            var physicalFiles = files.Where(File.Exists).ToArray();
            var cacheDependency = (physicalFiles.Length > 0 && !debuggingEnabled)
                ? new CacheDependency(physicalFiles)
                : null;
            HttpRuntime.Cache.Add(cacheKey, content, cacheDependency,
                Cache.NoAbsoluteExpiration, 
                new TimeSpan(365, 0, 0, 0),
                CacheItemPriority.NotRemovable,
                null);
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(BuildCacheKey(key));
        }

        private string BuildCacheKey(string key)
        {
            return KeyPrefix + key;
        }
    }
}