using System.Collections.Generic;

namespace Bundler.Framework.Directories
{
    public interface IDirectoryEnumerator
    {
        IEnumerable<string> GetFiles(string path);
    }
}