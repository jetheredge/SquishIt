using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace SquishIt.Framework.JavaScript
{
    public class BundleCache
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();

        public string GetContent(string name)
        {
            if (HttpContext.Current != null)
            {
                return (string)HttpContext.Current.Cache["squishit_" + name];
            }
            return cache[name];
        }

        public void ClearTestingCache()
        {
            cache.Clear();
        }

        public bool ContainsKey(string key)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Cache["squishit_" + key] != null;
            }
            return cache.ContainsKey(key);
        }

        public void AddToCache(string key, string content, List<string> files)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Add("squishit_" + key, content, new CacheDependency(files.ToArray()),
                                                Cache.NoAbsoluteExpiration, 
                                                new TimeSpan(365, 0, 0, 0),
                                                CacheItemPriority.NotRemovable,
                                                null);
            }
            else
            {
                cache.Add(key, content);    
            }
        }
    }
}