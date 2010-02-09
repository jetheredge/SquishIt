using System.Collections.Generic;

namespace JavascriptBundler.Directories
{
    public interface IDirectory
    {
        IEnumerable<string> GetFiles(string path, string js);
    }
}