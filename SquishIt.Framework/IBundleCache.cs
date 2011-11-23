using System.Collections.Generic;

namespace SquishIt.Framework
{
	public interface IBundleCache
	{
		string GetContent(string name);
		void ClearTestingCache();
		bool ContainsKey(string key);
		bool TryGetValue(string key, out string content);
		void Add(string key, string content, List<string> files);
	    void Remove(string key);
	}
}