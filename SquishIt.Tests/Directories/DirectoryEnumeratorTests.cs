using System;
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
            var temporaryDirectory = Path.GetTempPath() + "\\" + Path.GetRandomFileName() + "\\";
            System.IO.Directory.CreateDirectory(temporaryDirectory);
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    File.CreateText(temporaryDirectory + Path.GetRandomFileName() + ".js").Dispose();
                }                    

                var directoryEnumerator = new DirectoryEnumerator();
                var temporaryFiles = directoryEnumerator.GetFiles(temporaryDirectory).ToList();
                Assert.AreEqual(3, temporaryFiles.Count);
            }
            finally
            {
                System.IO.Directory.Delete(temporaryDirectory, true);
            }
        }
    }
}