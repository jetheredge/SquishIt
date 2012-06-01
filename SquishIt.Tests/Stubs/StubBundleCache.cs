using System.Collections.Generic;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs
{
	public class StubBundleCache: IBundleCache
	{
		Dictionary<string, string> cache = new Dictionary<string, string>();

		public string GetContent(string name)
		{
			return cache[name];
		}

		public void ClearTestingCache()
		{
			cache = new Dictionary<string, string>();
		}

		public bool ContainsKey(string key)
		{
			return cache.ContainsKey(key);
		}

		public bool TryGetValue(string key, out string content)
		{
			content = null;
			if (key == null)
				return false;

			return cache.TryGetValue(key, out content);
		}

		public void Add(string key, string content, List<string> files)
		{
			if (key != null)
				cache.Add(key, content);
		}

	    public void Remove(string key)
	    {
	        cache.Remove(key);
	    }
	}
}