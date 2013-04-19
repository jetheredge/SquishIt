using System.Collections.Generic;
using System.IO;

namespace SquishIt.Framework.Utilities
{
    public static class TempFileResolutionCache
    {
        static Dictionary<string, string> _resolutions = new Dictionary<string, string>();
 
        public static bool TryGetValue(string key, out string value)
        {
            if (_resolutions.TryGetValue(key, out value))
            {
                return true;
            }
            return false;
        }

        public static void Add(string key, string value)
        {
            _resolutions.Add(key, value);
        }

        public static void Clear()
        {
            foreach (var path in _resolutions.Values)
            {
                File.Delete(path);
            }

            _resolutions.Clear();
        }
    }
}
