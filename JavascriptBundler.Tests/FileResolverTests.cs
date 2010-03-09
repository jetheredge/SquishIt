using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bundler.Framework.FileResolvers;
using Bundler.Framework.Tests.Mocks;
using NUnit.Framework;

namespace Bundler.Tests
{
    [TestFixture]
    public class FileResolverTests
    {                
        [Test]
        public void CanResolveFile()
        {
            //resharper doesn't support the TestCase attribute
            var values = new Dictionary<string, string>()
                             {
                                 {@"C:\testfile.js", @"C:\testfile.js"},
                                 {@"C:\test\testfile.js", @"C:\test\testfile.js"},
                                 {@"D:\testfile.js", @"D:\testfile.js"},
                                 {@"\testfile.js", @"C:\testfile.js"},
                                 {@"\test\testfile.js", @"C:\test\testfile.js"},
                                 {@"\test\test3\testfile.js", @"C:\test\test3\testfile.js"},
                                 {@"testfile.js", Environment.CurrentDirectory + @"\testfile.js"},
                                 {@"..\testfile.js", Path.GetFullPath(Environment.CurrentDirectory + @"\..\testfile.js")},
                                 {@"..\..\testfile.js", Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\testfile.js")}
                             };

            var fileResolver = new FileResolver();
            foreach (string key in values.Keys)
            {
                var resolvedFile = fileResolver.TryResolve(key).ToList();
                Assert.AreEqual(values[key], resolvedFile[0]);
            }            
        }

        [Test]
        public void CanResolveDirectory()
        {
            var directoryEnumerator = new MockDirectoryEnumerator();
            var fileResolver = new DirectoryResolver(directoryEnumerator);
            var files = fileResolver.TryResolve(@"C:\test\").ToList();

            Assert.AreEqual(@"C:\test\file1.js", files[0]);
            Assert.AreEqual(@"C:\test\file2.js", files[1]);
            Assert.AreEqual(@"C:\test\file3.js", files[2]);
            Assert.AreEqual(@"C:\test\file4.js", files[3]);
            Assert.AreEqual(@"C:\test\file5.js", files[4]);
        }
    }
}