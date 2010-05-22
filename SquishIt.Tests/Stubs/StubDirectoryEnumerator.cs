using System.Collections.Generic;
using SquishIt.Framework.Directories;

namespace SquishIt.Tests.Stubs
{
    public class StubDirectoryEnumerator: IDirectoryEnumerator
    {
        public IEnumerable<string> GetFiles(string path)
        {
            yield return path + "file1.js";
            yield return path + "file2.js";
            yield return path + "file3.js";
            yield return path + "file4.js";
            yield return path + "file5.js";
        }
    }
}