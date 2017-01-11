using System;
using NUnit.Framework;
using SquishIt.Framework;

namespace SquishIt.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Apply()
        {
            ISquishItOptions capturedConfig = null;

            Action<ISquishItOptions> transformer = iso => { capturedConfig = iso; };

            Configuration.Apply(transformer);

            Assert.AreEqual(Configuration.Instance, capturedConfig);
        }
    }
}
