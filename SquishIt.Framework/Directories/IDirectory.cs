using System.Collections.Generic;

namespace SquishIt.Framework.Directories
{
    public interface IDirectory
    {
        IEnumerable<string> GetFiles(string path, string js);
    }
}