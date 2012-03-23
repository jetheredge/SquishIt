using System.Linq;
using System.Text.RegularExpressions;
using SquishIt.Framework;

namespace SquishIt.Sass
{
    public class SassPreprocessor : IPreprocessor
    {
        private static string[] extensions = new[] { ".sass", ".scss" };
        private static Regex sassFiles = new Regex(@"(\.sass)|(\.scss)$", RegexOptions.Compiled);

        private static Regex isSass = new Regex(@"\.sass$", RegexOptions.Compiled);
        private static Regex isScss = new Regex(@"\.scss$", RegexOptions.Compiled);

        public bool ValidFor(string filePath)
        {
            return sassFiles.IsMatch(filePath);
        }

        public string Process(string filePath, string content)
        {
            var compiler = new SassCompiler("");
            var sassMode = isSass.IsMatch(filePath) ? SassCompiler.SassMode.Sass : SassCompiler.SassMode.Scss;
            return compiler.CompileSass(content, sassMode);
        }

        public string[] Extensions
        {
            get { return extensions.Select(ext => ext.ToUpper()).ToArray(); }
        }
    }
}
