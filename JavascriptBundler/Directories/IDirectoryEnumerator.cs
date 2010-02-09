using System.Collections.Generic;

namespace JavascriptBundler.Directories
{
    public interface IDirectoryEnumerator
    {
        IEnumerable<string> GetFiles(string path);
    }
}