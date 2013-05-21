using System;
using NUnit.Framework;
using SquishIt.Framework.Invalidation;

namespace SquishIt.Tests
{
    [TestFixture]
    public class DefaultCacheInvalidationStrategyTests
    {
        [Test]
        public void GetOutputFileLocation()
        {
            var fileLocation = Guid.NewGuid().ToString();
            var hash = "HASH";

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(fileLocation, strategy.GetOutputFileLocation(fileLocation, hash));
        }

        [Test]
        public void GetOutputFileLocation_HashInFileName()
        {
            var fileLocation = Guid.NewGuid().ToString() + "_#";
            var hash = "HASH";

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(fileLocation.Replace("#", hash), strategy.GetOutputFileLocation(fileLocation, hash));
        }

        [Test]
        public void GetOutputWebPath_NoHash()
        {
            var renderToPath = Guid.NewGuid().ToString();

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath, strategy.GetOutputWebPath(renderToPath, null, null));
        }

        [Test]
        public void GetOutputWebPath_HashInFilename()
        {
            var renderToPath = Guid.NewGuid().ToString() + "_#";
            var hash = Guid.NewGuid().ToString();

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath.Replace("#", hash), strategy.GetOutputWebPath(renderToPath, null, hash));
        }

        [Test]
        public void GetOutputWebPath_NoHashKeyName()
        {
            var renderToPath = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath, strategy.GetOutputWebPath(renderToPath, null, hash));
        }

        [Test]
        public void GetOutputWebPath_No_Querystring()
        {
            var renderToPath = Guid.NewGuid().ToString();
            var hashKeyName = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath + "?" + hashKeyName + "=" + hash, strategy.GetOutputWebPath(renderToPath, hashKeyName, hash));
        }

        [Test]
        public void GetOutputWebPath_Querystring()
        {
            var renderToPath = Guid.NewGuid().ToString() + "?something=somethingelse";
            var hashKeyName = Guid.NewGuid().ToString();
            var hash = Guid.NewGuid().ToString();

            var strategy = new DefaultCacheInvalidationStrategy();

            Assert.AreEqual(renderToPath + "&" + hashKeyName + "=" + hash, strategy.GetOutputWebPath(renderToPath, hashKeyName, hash));
        }
    }
}
