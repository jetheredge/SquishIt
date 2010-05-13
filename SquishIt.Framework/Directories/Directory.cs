using System.Collections.Generic;

namespace SquishIt.Framework.Directories
{
    public class Directory: IDirectory
    {
        public IEnumerable<string> GetFiles(string path, string js)
        {
            return System.IO.Directory.GetFiles(path, js);
        }
    }
}