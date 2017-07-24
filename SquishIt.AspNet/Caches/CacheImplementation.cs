using System;
using System.Web;
using System.Web.Caching;
using SquishIt.Framework.Caches;

namespace SquishIt.AspNet.Caches
{
    public class CacheImplementation : ICacheImplementation
    {
        public object Add(string key, object value, string[] fileDependencies, bool debuggingEnabled)
        {
            var cacheDependency = (fileDependencies.Length > 0 && !debuggingEnabled)
                ? new CacheDependency(fileDependencies)
                : null;

            return HttpRuntime.Cache.Add(key, value, cacheDependency,
                Cache.NoAbsoluteExpiration,
                new TimeSpan(365, 0, 0, 0),
                CacheItemPriority.NotRemovable,
                null);
        }

        public bool ContainsKey(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }

        public object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}