using System;
using System.Collections.Generic;
using Bundler.Framework.Directories;

namespace Bundler.Framework.Tests.Mocks
{
    public class MockDirectoryEnumerator: IDirectoryEnumerator
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