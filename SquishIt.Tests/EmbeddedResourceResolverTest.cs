using System;
using System.IO;
using NUnit.Framework;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests
{
    [TestFixture]
    public class EmbeddedResourceResolverTest
    {
        const string cssContent = @"li {
    margin-bottom:0.1em;
    margin-left:0;
    margin-top:0.1em;
}

th {
    font-weight:normal;
    vertical-align:bottom;
}

.FloatRight {
    float:right;
}
                                
.FloatLeft {
    float:left;
}";

        [Test]
        public void CanResolveResource_Standard()
        {
            var resourcePath = "SquishIt.Tests://EmbeddedResource.Embedded.css";

            var embeddedResourceResolver = new StandardEmbeddedResourceResolver();

            var path = embeddedResourceResolver.Resolve(resourcePath);

            Assert.AreEqual(cssContent, File.ReadAllText(path));

            TempFileResolutionCache.Clear();
            
            Assert.False(File.Exists(path));
        }

        [Test]
        public void CanResolveResource_Standard_Reuses_Previous_Temp_File()
        {
            var resourcePath = "SquishIt.Tests://EmbeddedResource.Embedded.css";

            var embeddedResourceResolver = new StandardEmbeddedResourceResolver();

            var path = embeddedResourceResolver.Resolve(resourcePath);
            var path2 = embeddedResourceResolver.Resolve(resourcePath);

            Assert.AreEqual(cssContent, File.ReadAllText(path));
            Assert.AreEqual(cssContent, File.ReadAllText(path2));

            Assert.AreEqual(path, path2);

            TempFileResolutionCache.Clear();

            Assert.False(File.Exists(path));
        }

        [Test]
        public void CanResolveResource_Root()
        {
            var resourcePath = "SquishIt.Tests://RootEmbedded.css";

            var embeddedResourceResolver = new RootEmbeddedResourceResolver();

            var path = embeddedResourceResolver.Resolve(resourcePath);

            Assert.AreEqual(cssContent, File.ReadAllText(path));

            TempFileResolutionCache.Clear();

            Assert.False(File.Exists(path));
        }

        [Test]
        public void CanResolveResource_Root_Reuses_Previous_Temp_File()
        {
            var resourcePath = "SquishIt.Tests://RootEmbedded.css";

            var embeddedResourceResolver = new RootEmbeddedResourceResolver();

            var path = embeddedResourceResolver.Resolve(resourcePath);
            var path2 = embeddedResourceResolver.Resolve(resourcePath);

            Assert.AreEqual(cssContent, File.ReadAllText(path));
            Assert.AreEqual(cssContent, File.ReadAllText(path2));

            Assert.AreEqual(path, path2);

            TempFileResolutionCache.Clear();

            Assert.False(File.Exists(path));
        }

        [Test]
        public void ResolveFolder_Standard()
        {
            var resolver = new StandardEmbeddedResourceResolver();

            var ex = Assert.Throws<NotImplementedException>(() => resolver.ResolveFolder("", false, "", new string[0], new string[0]));

            Assert.AreEqual("Adding entire directories only supported by FileSystemResolver.", ex.Message);
        }

        [Test]
        public void ResolveFolder_Root()
        {
            var resolver = new RootEmbeddedResourceResolver();

            var ex = Assert.Throws<NotImplementedException>(() => resolver.ResolveFolder("", false, "", new string[0], new string[0]));

            Assert.AreEqual("Adding entire directories only supported by FileSystemResolver.", ex.Message);
        }
    }
}
