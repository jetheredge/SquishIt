using Moq;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.Utilities;
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
            hasher.Setup(h => h.GetHash(It.IsAny<string>())).Returns("pizza");

            var configuration = new Configuration().UseHasher(hasher.Object);

            using (new ConfigurationScope(configuration))
            {
                var jsTag = Bundle.JavaScript()
                                .AddString("test js")
                                .Render("test.js");

                Assert.True(jsTag.Contains("?r=pizza"));

                var cssTag = Bundle.Css()
                                   .AddString("test css")
                                   .Render("test.css");

                Assert.True(cssTag.Contains("?r=pizza"));
            }
            
            hasher.Verify(h => h.GetHash(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
