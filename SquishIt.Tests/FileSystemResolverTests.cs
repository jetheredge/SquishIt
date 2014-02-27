using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SquishIt.Framework.Resolvers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class FileSystemResolverTests
    {
        [Test, Platform(Exclude = "Unix, Linux, Mono")]
        public void CanResolveFile()
        {
            //dealing with working directory on a non-C location
            var currentDirectory = Environment.CurrentDirectory;
            var driveLetter = currentDirectory.Substring(0, currentDirectory.IndexOf("\\"));//ignoring UNC paths

            //resharper doesn't support the TestCase attribute
            var values = new Dictionary<string, string>
                             {
                                 {@"C:\testfile.js", @"C:\testfile.js"},
                                 {@"C:\test\testfile.js", @"C:\test\testfile.js"},
                                 {@"D:\testfile.js", @"D:\testfile.js"},
                                 {@"\testfile.js", driveLetter + @"\testfile.js"},
                                 {@"\test\testfile.js", driveLetter + @"\test\testfile.js"},
                                 {@"\test\test3\testfile.js", driveLetter + @"\test\test3\testfile.js"},
                                 {@"testfile.js", Environment.CurrentDirectory + @"\testfile.js"},
                                 {@"..\testfile.js", Path.GetFullPath(Environment.CurrentDirectory + @"\..\testfile.js")},
                                 {@"..\..\testfile.js", Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\testfile.js")}
                             };

            var fileResolver = new FileSystemResolver();
            foreach (string key in values.Keys)
            {
                var resolvedFile = fileResolver.Resolve(key);
                Assert.AreEqual(values[key], resolvedFile, key);
            }
        }

        [Test, Platform(Include = "Unix, Linux, Mono")]
        public void CanResolveFile_Unix()
        {
            var currentDirectory = Environment.CurrentDirectory;

            var values = new Dictionary<string, string>
                             {
                                 {@"testfile.js", Path.Combine(currentDirectory, "testfile.js")},
                                 {@"/testfile.js", @"/testfile.js"},
								 {@"../testfile.js", Path.Combine(currentDirectory.Substring(0, currentDirectory.LastIndexOf("/")), "testfile.js")}
                             };

            var fileResolver = new FileSystemResolver();
            foreach (string key in values.Keys)
            {
                var resolvedFile = fileResolver.Resolve(key);
                Assert.AreEqual(values[key], resolvedFile, key);
            }
        }

        [Test]
        public void CanResolveDirectory()
        {
            string path = Guid.NewGuid().ToString();
            var directory = Directory.CreateDirectory(path);

            try 
            {
                File.Create(Path.Combine(directory.FullName, "file1")).Close();
                File.Create(Path.Combine(directory.FullName, "file2")).Close();

                var result = new FileSystemResolver().ResolveFolder(path, true, Guid.NewGuid().ToString(), null, null).ToList();
                Assert.AreEqual(2, result.Count);
                Assert.Contains(path + Path.DirectorySeparatorChar + "file1", result);
                Assert.Contains(path + Path.DirectorySeparatorChar + "file2", result);
            }
            finally 
            {
                Directory.Delete(path, true);
            }
        }

        [Test]
        public void CanResolveDirectory_Filters_Files_By_Extension()
        {
            var path = Guid.NewGuid().ToString();
            var directory = Directory.CreateDirectory(path);

            try
            {
                File.Create(Path.Combine(directory.FullName, "file1.js")).Close();
                File.Create(Path.Combine(directory.FullName, "file2.css")).Close();
                File.Create(Path.Combine(directory.FullName, "file21.JS")).Close();

                var result = new FileSystemResolver().ResolveFolder(path, true, Guid.NewGuid().ToString(), new[] { ".js" }, new[] { ".css" }).ToList();
                Assert.AreEqual(2, result.Count);
                Assert.Contains(path + Path.DirectorySeparatorChar + "file1.js", result);
                Assert.Contains(path + Path.DirectorySeparatorChar + "file21.JS", result);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        [Test]
        public void CanResolveDirectory_Excludes_Debug_Files()
        {
            var path = Guid.NewGuid().ToString();
            var directory = Directory.CreateDirectory(path);
            var debugFileExtension = ".test.this.out";
            try
            {
                File.Create(Path.Combine(directory.FullName, "file1.js")).Close();
                File.Create(Path.Combine(directory.FullName, "file2.css")).Close();
                File.Create(Path.Combine(directory.FullName, "asdf.JS")).Close();
                File.Create(Path.Combine(directory.FullName, "thisoneshouldbeexccluded" + debugFileExtension)).Close();

                var result = new FileSystemResolver().ResolveFolder(path, true, debugFileExtension, new[] { ".js" }, new[] { ".css" }).ToList();
                Assert.AreEqual(2, result.Count);
                Assert.Contains(path + Path.DirectorySeparatorChar + "file1.js", result);
                Assert.Contains(path + Path.DirectorySeparatorChar + "asdf.JS", result);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        [Test]
        public void IsDirectory() 
        {
            var path = Guid.NewGuid().ToString();
            var directory = Directory.CreateDirectory(path);
            File.Create(Path.Combine(directory.FullName, "file")).Close();

            try 
            {
                var resolver = new FileSystemResolver();
                Assert.IsTrue(resolver.IsDirectory(path));
                Assert.IsFalse(resolver.IsDirectory(Path.Combine(path, "file")));
            }
            finally 
            {
                Directory.Delete(path, true);
            }
        }
    }
}