using System;
using NUnit.Framework;

namespace SquishIt.Tests
{
    [TestFixture]
    public class VersionTests
    {
        [TestCase("1.0.0", "1.0.0", true)]
        [TestCase("1.0.0", "1.0.1", true)]
        [TestCase("1.0.0", "1.1.0", true)]
        [TestCase("1.0.0", "2.0.0", true)]
        [TestCase("1.0.0", "0.0.1", false)]
        [TestCase("1.2.0", "1.1.0", false)]
        [TestCase("1.0.1", "1.0.0", false)]
        public void LessThanOrEqual(string v1, string v2, bool result)
        {
            var version1 = new Version(v1);
            var version2 = new Version(v2);

            Assert.AreEqual(result, version1 <= version2);
        }

        [TestCase("1.0.0", "1.0.0", true)]
        [TestCase("1.0.1", "1.0.0", true)]
        [TestCase("1.1.0", "1.0.0", true)]
        [TestCase("2.0.0", "1.0.0", true)]
        [TestCase("0.0.1", "1.0.0", false)]
        [TestCase("1.1.0", "1.2.0", false)]
        [TestCase("1.0.0", "1.0.1", false)]
        public void GreaterThanOrEqual(string v1, string v2, bool result)
        {
            var version1 = new Version(v1);
            var version2 = new Version(v2);

            Assert.AreEqual(result, version1 >= version2);
        }

        [TestCase("1.0.0", "1.0.0", false)]
        [TestCase("1.0.0", "1.0.1", true)]
        [TestCase("1.0.0", "1.1.0", true)]
        [TestCase("1.0.0", "2.0.0", true)]
        [TestCase("1.0.0", "0.0.1", false)]
        [TestCase("1.2.0", "1.1.0", false)]
        [TestCase("1.0.1", "1.0.0", false)]
        public void LessThan(string v1, string v2, bool result)
        {
            var version1 = new Version(v1);
            var version2 = new Version(v2);

            Assert.AreEqual(result, version1 < version2);
        }

        [TestCase("1.0.0", "1.0.0", false)]
        [TestCase("1.0.1", "1.0.0", true)]
        [TestCase("1.1.0", "1.0.0", true)]
        [TestCase("2.0.0", "1.0.0", true)]
        [TestCase("0.0.1", "1.0.0", false)]
        [TestCase("1.1.0", "1.2.0", false)]
        [TestCase("1.0.0", "1.0.1", false)]
        public void GreaterThan(string v1, string v2, bool result)
        {
            var version1 = new Version(v1);
            var version2 = new Version(v2);

            Assert.AreEqual(result, version1 > version2);
        }
    }
}
