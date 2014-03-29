using System.Web;
using Moq;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void WithHasher()
        {
            var hasher = new Mock<IHasher>();
            hasher.Setup(h => h.GetHash(It.IsAny<string>()))
                .Returns("pizza");

            var configuration = new Configuration().UseHasher(hasher.Object);

            var trustLevel = new Mock<ITrustLevel>();
            trustLevel.SetupGet(tl => tl.CurrentTrustLevel).Returns(AspNetHostingPermissionLevel.Unrestricted); //globally configured hasher is used another 2 times in high / full trust when obtaining mutex

            using(new ConfigurationScope(configuration))
            using(new TrustLevelScope(trustLevel.Object))
            {
                FilePathMutexProvider.instance = null;

                var jsTag = Bundle.JavaScript()
                                .AddString("test")
                                .Render("configured-hash.js");

                Assert.True(jsTag.Contains("?r=pizza"));

                var cssTag = Bundle.Css()
                                   .AddString("test")
                                   .Render("configured-hash.css");

                Assert.True(cssTag.Contains("?r=pizza"));
            }

            hasher.Verify(f => f.GetHash(It.Is<string>(s => s.EndsWith(".js"))), Times.Once());//js bundle rendering mutex
            hasher.Verify(h => h.GetHash(It.Is<string>(s => s.StartsWith("test;"))), Times.Once());//js content
            
            hasher.Verify(f => f.GetHash(It.Is<string>(s => s.EndsWith(".css"))), Times.Once());//css bundle rendering mutex
            hasher.Verify(h => h.GetHash("test"), Times.Once());//css content

        }
    }
}
