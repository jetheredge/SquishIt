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
    public class SHA1HasherTests
    {
        [TestCase("Bacon ipsum dolor sit amet strip steak tail andouille, short loin ham hock short ribs ball tip turkey shankle", "7E62873229AFB8368F0198FC99BF2F37BBF87D19")]
        [TestCase("", "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709")]
        public void HashString(string content, string expectedHash)
        {
            var SHA1Hasher = new SHA1Hasher(new Mock<IRetryableFileOpener>(MockBehavior.Strict).Object);

            var hash = SHA1Hasher.GetHash(content);

            Assert.AreEqual(expectedHash, hash);
        }

        [Test]
        public void HashString_NullContent()
        {
            var SHA1Hasher = new SHA1Hasher(new Mock<IRetryableFileOpener>(MockBehavior.Strict).Object);

            var ex = Assert.Throws<InvalidOperationException>(() => SHA1Hasher.GetHash((string)null));

            Assert.AreEqual("Can't calculate hash for null content.", ex.Message);
        }

        [TestCase("Bacon ipsum dolor sit amet strip steak tail andouille, short loin ham hock short ribs ball tip turkey shankle")]
        public void HashFileInfo(string data)
        {
            var info = new FileInfo(data);

            var retryableFileOpener = new Mock<IRetryableFileOpener>(MockBehavior.Strict);
            retryableFileOpener.Setup(rfo => rfo.OpenFileStream(info, 5, FileMode.Open, FileAccess.Read, FileShare.Read))
                .Returns(new MemoryStream(Encoding.ASCII.GetBytes(data)));

            var SHA1Hasher = new SHA1Hasher(retryableFileOpener.Object);

            Assert.AreEqual(SHA1Hasher.GetHash(data), SHA1Hasher.GetHash(info));
        }

        [Test]
        public void HashFileInfo_FileDoesntExist()
        {
            var info = new FileInfo("fakeFile");
            var SHA1Hasher = new SHA1Hasher(new RetryableFileOpener());
            var ex = Assert.Throws<FileNotFoundException>(() => SHA1Hasher.GetHash(info));
            //mono 3 uses " instead of ' around file name
			Assert.True(ex.Message.StartsWith("Could not find file"));
			Assert.True(ex.Message.Contains(info.FullName));
        }
    }
}
