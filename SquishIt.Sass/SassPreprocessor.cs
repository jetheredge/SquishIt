using System.Text.RegularExpressions;
using SquishIt.Framework.Base;
using SquishIt.Framework;
using System.IO;

namespace SquishIt.Sass
{
    public class SassPreprocessor : Preprocessor
    {
        static readonly Regex IsSass = new Regex(@"\.sass$", RegexOptions.Compiled);

		public override IProcessResult Process(string filePath, string content)
        {
            var compiler = new SassCompiler("");
            var sassMode = IsSass.IsMatch(filePath) ? SassCompiler.SassMode.Sass : SassCompiler.SassMode.Scss;
            return new ProcessResult(compiler.CompileSass(content, sassMode, Path.GetDirectoryName(filePath)));
        }

        public override string[] Extensions
        {
            get { return new[] { ".sass", ".scss" }; }
        }
    }
}
