using System.Text.RegularExpressions;
using LibSassNet;

namespace SquishIt.Sass
{
    public class SassCompiler
    {
        private readonly ISassCompiler _compiler = new LibSassNet.SassCompiler();

        internal static string RootAppPath;

        public SassCompiler(string rootPath)
        {
            RootAppPath = rootPath;
        }

        public enum SassMode
        {
            Sass,
            Scss
        }

        public string CompileSass(string input, SassMode mode)
        {
            var processedInput = mode == SassMode.Scss ? input : ConvertToScss(input);
            return _compiler.Compile(processedInput, OutputStyle.Nested, SourceCommentsMode.None, null);
        }

        internal string ConvertToScss(string input)
        {
            throw new System.NotImplementedException("need to convert Sass to Scss, see http://sasstoscss.com/ maybe?");
        }
    }
}
