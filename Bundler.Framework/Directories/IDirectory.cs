using System.Collections.Generic;

namespace Bundler.Framework.Directories
{
    public interface IDirectory
    {
        IEnumerable<string> GetFiles(string path, string js);
    }
}