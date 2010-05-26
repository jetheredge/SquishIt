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
