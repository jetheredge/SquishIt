using NUnit.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CSSPathRewriterTests
    {
        [Test]
        public void CanRewritePathsInCssWhenAbsolutePathsAreUsed()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(/img/blah/somethingelse.jpg);
                                                        }
                                                    ";

            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenDifferentFoldersAtSameDepth()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\output.css");

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenMultipleOccurencesOfSameRelativePathAppearInOneCssFile()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .ui-icon { background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                                                    ";
            //real example from jquery ui-generated custom css file

            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\someothersubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .ui-icon { background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                                                    ";
            //if it fails, it will look like this: url(someothersubpath/someothersubpath/someothersubpath/images/

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenMultipleOccurencesOfSameRelativePathAppearInOneCssFileWithDifferentCasing()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .ui-icon { background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(Images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(iMages/ui-icons_222222_256x240.png); }
                                                    ";

            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\someothersubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .ui-icon { background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(someothersubpath/Images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(someothersubpath/iMages/ui-icons_222222_256x240.png); }
                                                    ";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenOutputFolderDeeper()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(../../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenOutputFolderMoreShallow()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenRelativePathsInsideOfSourceFolder()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(../../somesubpath/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../../somesubpath/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWithQuotes()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(""../img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: url(""../img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(""img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: url(""img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWithSingleQuotes()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url('../img/something.jpg');
                                                        }

                                                        .footer {
                                                                background-image: url('../img/blah/somethingelse.jpg');
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url('img/something.jpg');
                                                        }

                                                        .footer {
                                                                background-image: url('img/blah/somethingelse.jpg');
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWithSpaces()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url( ""../img/something.jpg"" );
                                                        }

                                                        .footer {
                                                                background-image: url( ""../img/blah/somethingelse.jpg"" );
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url( ""img/something.jpg"" );
                                                        }

                                                        .footer {
                                                                background-image: url( ""img/blah/somethingelse.jpg"" );
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWithUppercaseUrlStatement()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: URL(""../img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: uRL(""../img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: URL(""img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: uRL(""img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DontThrowIfPathContainsRegexMetacharacters()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: URL(""*../img/something.jpg"");
                                                                background-image: url(c:\documents\usr8\local\temp\1\d1b73b93-5ff0-11de-9339-0017317c60aa); 
                                                        }

                                                        .footer {
                                                                background-image: uRL(""..c:\1\2\somethingelse.jpg"");
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");

            //concrete result doesn't matter here (since input isn't valid)
            //the only important thing is that it shouldn't throw exceptions
            Assert.DoesNotThrow(() => CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher));
        }

        [Test]
        public void DontThrowIfPathIsEmpty()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: URL("""");
                                                                background-image: url(); 
                                                        }

                                                        .footer {
                                                                background-image: uRL("""");
                                                                background-image: UrL('');
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");

            //concrete result doesn't matter here (since input isn't valid)
            //the only important thing is that it shouldn't throw exceptions
            Assert.DoesNotThrow(() => CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher));
        }

        [Test]
        public void WontRewriteAbsolutePaths()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(http://www.somewhere.com/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(http://www.somewhere.com/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background-image: url(http://www.somewhere.com/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(http://www.somewhere.com/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WontRewriteDataUrls()
        {
            

            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUg...') no-repeat 0 0;
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUg...') no-repeat 0 0;
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WontRewriteBehaviorUrls()
        {
            ICSSAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                behavior: url('somethingorother') no-repeat 0 0;
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\someothersubpath\evendeeper\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

            string expected =
                @"
                                                        .header {
                                                                behavior: url('somethingorother') no-repeat 0 0;
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WontEncodeHashesOrQuestionMarks()
        {
            var css = @"@font-face {
font-family: 'Museo';
src: url('../case/Museo.eot');
src: url('../case/Museo.eot?#iefix') format('embedded-opentype'), url('../case/Museo.woff') format('woff'), url('../case/Museo.ttf') format('truetype'), url('../case/Museo.svg#Museo') format('svg');
font-weight: normal;
font-style: normal;
}";

            var sourceFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myFile.css");
            var destinationFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myRewrittenFile.css");

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null);

            Assert.AreEqual(css, result);
        }
    }
}