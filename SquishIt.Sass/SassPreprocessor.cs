using System.Text.RegularExpressions;
using SquishIt.Framework.Base;

namespace SquishIt.Sass
{
    public class SassPreprocessor : Preprocessor
    {
        static readonly Regex IsSass = new Regex(@"\.sass$", RegexOptions.Compiled);

        public override string Process(string filePath, string content)
        {
            var compiler = new SassCompiler("");
            var sassMode = IsSass.IsMatch(filePath) ? SassCompiler.SassMode.Sass : SassCompiler.SassMode.Scss;
            return compiler.CompileSass(content, sassMode);
        }

        public override string[] Extensions
        {
            get { return new[] { ".sass", ".scss" }; }
        }
    }
}
