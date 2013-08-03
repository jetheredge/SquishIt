using System;
using System.IO;
using NUnit.Framework;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class HttpResolverTest
    {
        string _htmlContent = TestUtilities.NormalizeLineEndings(@"<!doctype html>
<html>
<head>
    <title>Example Domain</title>

    <meta charset=""utf-8"" />
    <meta http-equiv=""Content-type"" content=""text/html; charset=utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <style type=""text/css"">
    body {
        background-color: #f0f0f2;
        margin: 0;
        padding: 0;
        font-family: ""Open Sans"", ""Helvetica Neue"", Helvetica, Arial, sans-serif;
        
    }
    div {
        width: 600px;
        margin: 5em auto;
        padding: 50px;
        background-color: #fff;
        border-radius: 1em;
    }
    a:link, a:visited {
        color: #38488f;
        text-decoration: none;
    }
    @media (max-width: 700px) {
        body {
            background-color: #fff;
        }
        div {
            width: auto;
            margin: 0 auto;
            border-radius: 0;
            padding: 1em;
        }
    }
    </style>    
</head>

<body>
<div>
    <h1>Example Domain</h1>
    <p>This domain is established to be used for illustrative examples in documents. You may use this
    domain in examples without prior coordination or asking for permission.</p>
    <p><a href=""http://www.iana.org/domains/example"">More information...</a></p>
</div>
</body>
</html>
");
        [Test]
        public void CanResolveResource()
        {
            var resourcePath = "http://example.com";

            var httpResolver = new HttpResolver();

            var path = httpResolver.Resolve(resourcePath);

            Assert.AreEqual(_htmlContent, File.ReadAllText(path));

            TempFileResolutionCache.Clear();
            
            Assert.False(File.Exists(path));
        }

        [Test]
        public void CanResolveResource_Reuses_Previous_Temp_File()
        {
            var resourcePath = "http://example.com";

            var embeddedResourceResolver = new HttpResolver();

            var path = embeddedResourceResolver.Resolve(resourcePath);
            var path2 = embeddedResourceResolver.Resolve(resourcePath);

            Assert.AreEqual(_htmlContent, File.ReadAllText(path));
            Assert.AreEqual(_htmlContent, File.ReadAllText(path2));

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
    }
}
