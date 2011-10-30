﻿using System;
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
                var resolvedFile = fileResolver.TryResolve(key).ToList();
                Assert.AreEqual(values[key], resolvedFile[0], key);
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
                var resolvedFile = fileResolver.TryResolve(key).ToList();
                Assert.AreEqual(values[key], resolvedFile[0], key);
            }
        }
    }
}