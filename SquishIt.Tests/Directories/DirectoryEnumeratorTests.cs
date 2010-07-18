using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SquishIt.Framework.Directories;

namespace SquishIt.Framework.Tests
{
    [TestFixture]
    public class DirectoryEnumeratorTests
    {
        [Test]
        public void CanEnumerateDirectory()
        {
            var temporaryDirectory = CreateNewTemporaryDirectory();
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateJsFileWithRandomName(temporaryDirectory);
                }                    

                var directoryEnumerator = new DirectoryEnumerator();
                var result = directoryEnumerator.GetFiles(temporaryDirectory).ToList();

                Assert.AreEqual(3, result.Count);
            }
            finally
            {
                System.IO.Directory.Delete(temporaryDirectory, true);
            }
        }

        [Test]
        public void EnumerationSkipsVsDocFiles()
        {
            var temporaryDirectory = CreateNewTemporaryDirectory();
            try
            {
                CreateJsFileWithRandomName(temporaryDirectory);
                CreateJsFileWithRandomNameAndSuffix(temporaryDirectory, "-vsdoc");

                var directoryEnumerator = new DirectoryEnumerator();
                var result = directoryEnumerator.GetFiles(temporaryDirectory).ToList();

                Assert.AreEqual(1, result.Count);
            }
            finally
            {
                System.IO.Directory.Delete(temporaryDirectory, true);
            }
        }

        [Test]
        public void CanOrderFilesInDirectory()
        {
            var temporaryDirectory = CreateNewTemporaryDirectory();
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    string fileName = String.Format("{0}File{1}.js", temporaryDirectory, (1 + i));
                    File.CreateText(fileName).Dispose();
                }
                using (var ordering = File.CreateText(temporaryDirectory + "ordering.txt"))
                {
                    ordering.WriteLine("File3.js");
                    ordering.WriteLine("File4.js");
                    ordering.WriteLine("File1.js");
                    ordering.WriteLine("File2.js");
                }

                var directoryEnumerator = new DirectoryEnumerator();
                var result = directoryEnumerator.GetFiles(temporaryDirectory).ToList();

                var expectedList = new List<string>
                {
                    temporaryDirectory + "File3.js",
                    temporaryDirectory + "File4.js",
                    temporaryDirectory + "File1.js",
                    temporaryDirectory + "File2.js"
                };
                CollectionAssert.AreEqual(expectedList, result, StringComparer.OrdinalIgnoreCase);
            }
            finally
            {
                System.IO.Directory.Delete(temporaryDirectory, true);
            }
        }

        private static string CreateNewTemporaryDirectory()
        {
            string temporaryDirectory = Path.GetTempPath() + "\\" + Path.GetRandomFileName() + "\\";
            System.IO.Directory.CreateDirectory(temporaryDirectory);
            return temporaryDirectory;
        }

        private static void CreateJsFileWithRandomName(string temporaryDirectory)
        {
            CreateJsFileWithRandomNameAndSuffix(temporaryDirectory, String.Empty);
        }

        private static void CreateJsFileWithRandomNameAndSuffix(string temporaryDirectory, string suffix)
        {
            CreateJsFile(temporaryDirectory, Path.GetRandomFileName() + suffix);
        }

        private static void CreateJsFile(string temporaryDirectory, string nameWithoutExtension)
        {
            string filePath = temporaryDirectory + nameWithoutExtension + ".js";
            File.CreateText(filePath).Dispose();
        }
    }
}