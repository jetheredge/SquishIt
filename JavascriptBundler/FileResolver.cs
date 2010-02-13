using System;
using System.Collections.Generic;
using System.IO;

namespace JavascriptBundler
{
    public class FileResolver
    {
        /*public IEnumerable<string> ResolveFromDirectories(IEnumerable<string> directories)
        {
        }*/
        
        /*public IEnumerable<string> ResolveFromDirectory(string directory)
        {
        }*/

        public string ResolveFromFile(string file)
        {
            return Path.GetFullPath(file);
        }
    }
}
