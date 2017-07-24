using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SquishIt.Framework.Caches
{
    public abstract class ContentCache : IContentCache
    {
        private readonly ICacheImplementation _cacheImplementation;

        public ContentCache()
        {
            _cacheImplementation = Configuration.Instance.DefaultCacheImplementation;
        }

        private readonly List<string> CacheKeys = new List<string>();
        protected abstract string KeyPrefix { get; }

        public string GetContent(string name)
        {
            return (string)_cacheImplementation.Get(BuildCacheKey(name));
        }

        public void ClearTestingCache()
        {
            foreach (var key in CacheKeys)
            {
                _cacheImplementation.Remove(key);
            }
        }

        public bool ContainsKey(string key)
        {
            return _cacheImplementation.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string content)
        {
            content = (string)_cacheImplementation.Get(BuildCacheKey(key));
            return content != null;
        }

        public void Add(string key, string content, List<string> files, bool debuggingEnabled)
        {
            var cacheKey = BuildCacheKey(key);
            CacheKeys.Add(cacheKey);

            var physicalFiles = files.Where(File.Exists).ToArray();

            _cacheImplementation.Add(cacheKey, content, physicalFiles, debuggingEnabled);
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