using System.IO;
using Moq;
using NUnit.Framework;
using SquishIt.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.Resolvers;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CSSPathRewriterTests
    {
        //TODO: mock path translators
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

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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

            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            Assert.DoesNotThrow(() => CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator()));
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
            Assert.DoesNotThrow(() => CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator()));
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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

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

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator());

            Assert.AreEqual(css, result);
        }

        [Test]
        public void WontEncodeHashesOrQuestionMarks_Import()
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

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator(), asImport: true);

            Assert.AreEqual(css.Replace("../", "squishit://../"), result);
        }

        [Test]
        public void WontEncodeSpecialCharacters()
        {
            var css = @".icon-facebook-squared:before { content: '\e804'; } /* '' */
.icon-calendar:before { content: '\e801'; } /* '' */";

            var sourceFile = TestUtilities.PreparePath(@"C:\somepath\myFile.css");
            var destinationFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myRewrittenFile.css");

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator());

            Assert.AreEqual(css, result);
        }

        [Test]
        public void FontsWith()
        {
            var css = @".icon-facebook-squared:before { content: '\e804'; } /* '' */
.icon-calendar:before { content: '\e801'; } /* '' */";

            var sourceFile = TestUtilities.PreparePath(@"C:\somepath\myFile.css");
            var destinationFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myRewrittenFile.css");

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator());

            Assert.AreEqual(css, result);
        }

        [Test]
        public void CanRewritePathsInCSSWhenSourceDeeperThanDestination()
        {
            //https://github.com/jetheredge/SquishIt/issues/264
            var css = @"background: transparent url('../Images/rss-icon.png') no-repeat;";

            var expected = @"background: transparent url('../../Themes/Metro/Content/Images/rss-icon.png') no-repeat;";

            var sourceFile = TestUtilities.PreparePath(@"C:\X\Themes\Metro\Content\Styles\myFile.css");
            var destinationFile = TestUtilities.PreparePath(@"C:\X\Content\cache\combined.css");

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator());

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AlwaysReplaceSquishItRewrittenPathsWhenProcessingParent()
        {
            //an issue was reported relating to squishit:// (rewritten urls for imports) not being scrubbed out if there are no relative paths in the parent file
            //https://github.com/jetheredge/SquishIt/issues/277
            var css = @"squishit://test";

            var sourceFile = TestUtilities.PreparePath(@"C:\somepath\myFile.css");
            var destinationFile = TestUtilities.PreparePath(@"C:\somepath\somesubpath\myRewrittenFile.css");

            var result = CSSPathRewriter.RewriteCssPaths(destinationFile, sourceFile, css, null, new PathTranslator());

            Assert.AreEqual(@"test", result);
        }

        [Test]
        public void WontThrowPathTooLongExceptionForLongDataUrls()
        {
            //
            // Base64 images can throw Windows PathTooLong exceptions (limit of 260 characters)
            // when present in a CSS file with non-base64 relative URLs.
            //

            var cssAssetsFileHasher = new CSSAssetsFileHasher(string.Empty, new FileSystemResolver(), null, null);
            
            string css =
                @"
                                                        .header {
                                                                background: url('data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/PgIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPGxpbmVhckdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjAlIiB5MT0iMCUiIHgyPSIwJSIgeTI9IjEwMCUiPgogICAgPHN0b3Agb2Zmc2V0PSIwJSIgc3RvcC1jb2xvcj0iI2ZlZmZmZiIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjEwMCUiIHN0b3AtY29sb3I9IiNmMmYyZjIiIHN0b3Atb3BhY2l0eT0iMSIvPgogIDwvbGluZWFyR3JhZGllbnQ+CiAgPHJlY3QgeD0iMCIgeT0iMCIgd2lkdGg9IjEiIGhlaWdodD0iMSIgZmlsbD0idXJsKCNncmFkLXVjZ2ctZ2VuZXJhdGVkKSIgLz4KPC9zdmc+') no-repeat 0 0;
                                                                background: url('fake.png');
                                                        }
                                                    ";
            string sourceFile = TestUtilities.PreparePath(@"C:\somepath\myfile.css");
            string targetFile = TestUtilities.PreparePath(@"C:\somepath\output.css");
            string result = CSSPathRewriter.RewriteCssPaths(targetFile, sourceFile, css, cssAssetsFileHasher, new PathTranslator());

            string expected =
                @"
                                                        .header {
                                                                background: url('data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/PgIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPGxpbmVhckdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjAlIiB5MT0iMCUiIHgyPSIwJSIgeTI9IjEwMCUiPgogICAgPHN0b3Agb2Zmc2V0PSIwJSIgc3RvcC1jb2xvcj0iI2ZlZmZmZiIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjEwMCUiIHN0b3AtY29sb3I9IiNmMmYyZjIiIHN0b3Atb3BhY2l0eT0iMSIvPgogIDwvbGluZWFyR3JhZGllbnQ+CiAgPHJlY3QgeD0iMCIgeT0iMCIgd2lkdGg9IjEiIGhlaWdodD0iMSIgZmlsbD0idXJsKCNncmFkLXVjZ2ctZ2VuZXJhdGVkKSIgLz4KPC9zdmc+') no-repeat 0 0;
                                                                background: url('fake.png');
                                                        }
                                                    ";
            Assert.AreEqual(expected, result);
        }
    }
}