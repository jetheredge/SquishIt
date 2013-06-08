using System;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests
{
    [TestFixture]
    public class HasherTests
    {
        [TestCase("Bacon ipsum dolor sit amet strip steak tail andouille, short loin ham hock short ribs ball tip turkey shankle", "88E4C6F3129A86EF8B3CC8457A99CF83")]
        [TestCase("", "D41D8CD98F00B204E9800998ECF8427E")]
        public void HashString(string content, string expectedHash)
        {
            var hasher = new Hasher(new Mock<IRetryableFileOpener>(MockBehavior.Strict).Object);

            var hash = hasher.GetHash(content);

            Assert.AreEqual(expectedHash, hash);
        }

        [Test]
        public void HashString_NullContent()
        {
            var hasher = new Hasher(new Mock<IRetryableFileOpener>(MockBehavior.Strict).Object);

            var ex = Assert.Throws<InvalidOperationException>(() => hasher.GetHash((string)null));

            Assert.AreEqual("Can't calculate hash for null content.", ex.Message);
        }

        [TestCase("Bacon ipsum dolor sit amet strip steak tail andouille, short loin ham hock short ribs ball tip turkey shankle")]
        public void HashFileInfo(string data)
        {
            var info = new FileInfo(data);

            var retryableFileOpener = new Mock<IRetryableFileOpener>(MockBehavior.Strict);
            retryableFileOpener.Setup(rfo => rfo.OpenFileStream(info, 5, FileMode.Open, FileAccess.Read, FileShare.Read))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(data)));

            var hasher = new Hasher(retryableFileOpener.Object);

            Assert.AreEqual(hasher.GetHash(data), hasher.GetHash(info));
        }

        [Test]
        public void HashFileInfo_FileDoesntExist()
        {
            var info = new FileInfo("fakeFile");
            var hasher = new Hasher(new RetryableFileOpener());
            var ex = Assert.Throws<FileNotFoundException>(() => hasher.GetHash(info));
            //mono 3 uses " instead of ' around file name
			Assert.True(ex.Message.StartsWith("Could not find file"));
			Assert.True(ex.Message.Contains(info.FullName));
        }
    }
}
