using NUnit.Framework;
using SquishIt.Framework.Css;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CssPathRewriterTests
    {
        [Test]
        public void CanRewritePathsInCssWhenOutputFolderMoreShallow()
        {
            string css = @"
                            .header {
                                background-image: url(../img/something.jpg);
                            }

                            .footer {
                                background-image: url(../img/blah/somethingelse.jpg);
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWithQuotes()
        {
            string css = @"
                            .header {
                                background-image: url(""../img/something.jpg"");
                            }

                            .footer {
                                background-image: url(""../img/blah/somethingelse.jpg"");
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWithUppercaseUrlStatement()
        {
            string css = @"
                            .header {
                                background-image: URL(""../img/something.jpg"");
                            }

                            .footer {
                                background-image: uRL(""../img/blah/somethingelse.jpg"");
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWhenDifferentFoldersAtSameDepth()
        {
            string css = @"
                            .header {
                                background-image: url(../img/something.jpg);
                            }

                            .footer {
                                background-image: url(../img/blah/somethingelse.jpg);
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWhenOutputFolderDeeper()
        {
            string css = @"
                            .header {
                                background-image: url(../img/something.jpg);
                            }

                            .footer {
                                background-image: url(../img/blah/somethingelse.jpg);
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWhenRelativePathsInsideOfSourceFolder()
        {
            string css = @"
                            .header {
                                background-image: url(img/something.jpg);
                            }

                            .footer {
                                background-image: url(img/blah/somethingelse.jpg);
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
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
        public void CanRewritePathsInCssWhenMultipleOccurencesOfSameRelativePathAppearInOneCssFile()
        {
            string css = @"
                            .ui-icon { background-image: url(images/ui-icons_222222_256x240.png); }
                            .ui-widget-content .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                            .ui-widget-header .ui-icon {background-image: url(images/ui-icons_222222_256x240.png); }
                          "; //real example from jquery ui-generated custom css file
            
            string sourceFile = @"C:\somepath\somesubpath\someothersubpath\myfile.css";
            string targetFile = @"C:\somepath\somesubpath\myfile.css";
            
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
                            .ui-icon { background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                            .ui-widget-content .ui-icon {background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                            .ui-widget-header .ui-icon {background-image: url(someothersubpath/images/ui-icons_222222_256x240.png); }
                          "; //if it fails, it will look like this: url(someothersubpath/someothersubpath/someothersubpath/images/

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanRewritePathsInCssWhenAbsolutePathsAreUsed()
        {
            string css = @"
                            .header {
                                background-image: url(/img/something.jpg);
                            }

                            .footer {
                                background-image: url(/img/blah/somethingelse.jpg);
                            }
                          ";
            string sourceFile = @"C:\somepath\somesubpath\myfile.css";
            string targetFile = @"C:\somepath\someothersubpath\evendeeper\output.css";
            string result = CssPathRewriter.RewriteCssPaths(targetFile, sourceFile, css);

            string expected = @"
                            .header {
                                background-image: url(/img/something.jpg);
                            }

                            .footer {
                                background-image: url(/img/blah/somethingelse.jpg);
                            }
                          ";
            Assert.AreEqual(expected, result);
        }
    }
}
