using LibSassNet;

namespace SquishIt.Sass
{
    public class SassCompiler
    {
        private readonly ISassCompiler _compiler = new LibSassNet.SassCompiler();
        private readonly ISassToScssConverter _converter = new SassToScssConverter();
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

        public string CompileSass(string input, SassMode mode, string location, int precision = 5)
        {
            var processedInput = mode == SassMode.Scss ? input : ConvertToScss(input);
            return _compiler.Compile(processedInput, OutputStyle.Nested, SourceCommentsMode.None, precision, new[] { location });
        }

        internal string ConvertToScss(string input)
        {
            return _converter.Convert(input);
        }
    }
}
