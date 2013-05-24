using System;
using NUnit.Framework;
using SquishIt.Framework.Invalidation;

namespace SquishIt.Tests
{
    [TestFixture]
    public class HashAsVirtualDirectoryCacheInvalidationStrategyTests
    {
        [Test]
        public void GetOutputFileLocation()
        {
            var fileLocation = Guid.NewGuid().ToString();
            var hash = "HASH";

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(fileLocation, strategy.GetOutputFileLocation(fileLocation, hash));
        }

        [Test]
        public void GetOutputFileLocation_HashInFolderName()
        {
            var fileLocation = "/#/" + Guid.NewGuid().ToString();
            var hash = "HASH";

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual("/" + fileLocation.TrimStart('/', '#'), strategy.GetOutputFileLocation(fileLocation, hash));
        }

        [Test]
        public void GetOutputWebPath_NoHash()
        {
            var renderToPath = Guid.NewGuid().ToString();

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath, strategy.GetOutputWebPath(renderToPath, null, null));
        }

        [Test]
        public void GetOutputWebPath_HashInFolderName()
        {
            var renderToPath = "/#/" + Guid.NewGuid().ToString();
            var hashKeyName = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath.Replace("#", hashKeyName + "-" + hash), strategy.GetOutputWebPath(renderToPath, hashKeyName, hash));
        }

        //still fall back to querystring
        [Test]
        public void GetOutputWebPath_NoHashKeyName()
        {
            var renderToPath = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath, strategy.GetOutputWebPath(renderToPath, null, hash));
        }

        [Test]
        public void GetOutputWebPath_No_Querystring()
        {
            var renderToPath = Guid.NewGuid().ToString();
            var hashKeyName = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath + "?" + hashKeyName + "=" + hash, strategy.GetOutputWebPath(renderToPath, hashKeyName, hash));
        }

        [Test]
        public void GetOutputWebPath_Querystring()
        {
            var renderToPath = Guid.NewGuid().ToString() + "?something=somethingelse";
            var hashKeyName = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new HashAsVirtualDirectoryCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath + "&" + hashKeyName + "=" + hash, strategy.GetOutputWebPath(renderToPath, hashKeyName, hash));
        }
    }
}
