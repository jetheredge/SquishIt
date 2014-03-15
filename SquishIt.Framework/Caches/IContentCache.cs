using System.Collections.Generic;

namespace SquishIt.Framework.Caches
{
	public interface IContentCache
	{
		string GetContent(string name);
		void ClearTestingCache();
		bool ContainsKey(string key);
		bool TryGetValue(string key, out string content);
		void Add(string key, string content, List<string> files, bool debuggingEnabled);
	    void Remove(string key);
	}
}