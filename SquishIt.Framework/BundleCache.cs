using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SquishIt.Framework
{
    public class BundleCache: IBundleCache
    {
        const string KEY_PREFIX = "squishit_";

        readonly List<string> CacheKeys = new List<string>();

        public string GetContent(string name)
        {
            return (string)HttpRuntime.Cache[KEY_PREFIX + name];
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
            return HttpRuntime.Cache[KEY_PREFIX + key] != null;
        }

        public bool TryGetValue(string key, out string content)
        {
            content = (string)HttpRuntime.Cache[KEY_PREFIX + key];
            return content != null;
        }

        public void Add(string key, string content, List<string> files)
        {
            var cacheKey = KEY_PREFIX + key;
            CacheKeys.Add(cacheKey);

            var physicalFiles = files.Where(File.Exists).ToArray();
            var cacheDependency = physicalFiles.Length > 0
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
            HttpRuntime.Cache.Remove(key);
        }
    }
}