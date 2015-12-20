using System;
using System.IO;
using NUnit.Framework;
using SquishIt.Framework.Files;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class RetryableFileOpenerTest
    {
        [Test]
        public void OpenFileStream_Directory_Exists()
        {

            var folder = TestUtilities.PreparePath(Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString()));
            try
            {

                Directory.CreateDirectory(folder);

                var opener = new RetryableFileOpener();

                var filePath = Path.Combine(folder, "file.js");
                using (opener.OpenFileStream(filePath, 1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                }

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(folder,true);
            }
        }
        
        [Test]
        public void OpenFileStream_Directory_DoesNotExists_ShouldCreate()
        {

            var folder = TestUtilities.PreparePath(Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            try
            {

                var opener = new RetryableFileOpener();

                var filePath = Path.Combine(folder, "file.js");
                using (opener.OpenFileStream(filePath, 2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                }

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }


        [Test]
        public void OpenTextStreamWriter_Directory_Exists()
        {

            var folder = TestUtilities.PreparePath(Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString()));
            try
            {

                Directory.CreateDirectory(folder);

                var opener = new RetryableFileOpener();

                var filePath = Path.Combine(folder, "file.js");
                using (opener.OpenTextStreamWriter(filePath, 2, false))
                {
                }

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [Test]
        public void OpenTextStreamWriter_Directory_DoesNotExists_ShouldCreate()
        {

            var folder = TestUtilities.PreparePath(Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            try
            {

                var opener = new RetryableFileOpener();

                var filePath = Path.Combine(folder, "file.js");
                using (opener.OpenTextStreamWriter(filePath, 2, false))
                {
                }

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }
        
        [Test]
        public void OpenTextStreamWriter_Throws_Exception_With_Filepath()
        {
            var fakePath = Guid.NewGuid().ToString();

            var opener = new RetryableFileOpener();

            var ex = Assert.Throws<IOException>(() => opener.OpenTextStreamWriter(fakePath, 0, true));

            Assert.True(ex.Message.Contains(fakePath));
        }

        [Test]
        public void OpenTextStreamReader_Throws_Exception_With_Filepath()
        {
            var fakePath = Guid.NewGuid().ToString();

            var opener = new RetryableFileOpener();

            var ex = Assert.Throws<IOException>(() => opener.OpenTextStreamReader(fakePath, 0));

            Assert.True(ex.Message.Contains(fakePath));
        }

        [Test]
        public void OpenFileStream_Throws_Exception_With_Filepath()
        {
            var fakePath = Guid.NewGuid().ToString();

            var opener = new RetryableFileOpener();

            var ex = Assert.Throws<IOException>(() => opener.OpenFileStream(fakePath, 0, FileMode.Open, FileAccess.ReadWrite, FileShare.None));

            Assert.True(ex.Message.Contains(fakePath));
        }
    }
}
