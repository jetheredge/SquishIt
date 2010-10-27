using NUnit.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CssPathRewriterTests
    {
        [Test]
        public void CanRewritePathsInCssWhenAbsolutePathsAreUsed()
        {
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .ui-icon { background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                                                    ";
                //real example from jquery ui-generated custom css file

            string sourceFile = @"C:\somepath\somesubpath\someothersubpath\myfile.css";
            string targetFile = @"C:\somepath\somesubpath\myfile.css";

            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .ui-icon { background-image: url(images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-content .ui-icon {background-image: url(Images/ui-icons_222222_256x240.png); }
                                                        .ui-widget-header .ui-icon {background-image: url(iMages/ui-icons_222222_256x240.png); }
                                                    ";

            string sourceFile = @"C:\somepath\somesubpath\someothersubpath\myfile.css";
            string targetFile = @"C:\somepath\somesubpath\myfile.css";

            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(../img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(../img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(""../img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: url(""../img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url('../img/something.jpg');
                                                        }

                                                        .footer {
                                                                background-image: url('../img/blah/somethingelse.jpg');
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
        public void CanRewritePathsInCssWithUppercaseUrlStatement()
        {
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: URL(""../img/something.jpg"");
                                                        }

                                                        .footer {
                                                                background-image: uRL(""../img/blah/somethingelse.jpg"");
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
        public void WontRewriteAbsolutePaths()
        {
            ICssAssetsFileHasher cssAssetsFileHasher = null;
            string css =
                @"
                                                        .header {
                                                                background-image: url(http://www.somewhere.com/img/something.jpg);
                                                        }

                                                        .footer {
                                                                background-image: url(http://www.somewhere.com/img/blah/somethingelse.jpg);
                                                        }
                                                    ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher);

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
    }
}