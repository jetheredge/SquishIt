using System.Collections.Generic;

namespace SquishIt.Framework.Directories
{
    public interface IDirectoryEnumerator
    {
        IEnumerable<string> GetFiles(string path);
    }
}