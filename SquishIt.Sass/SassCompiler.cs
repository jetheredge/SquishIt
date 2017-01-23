using System;
using LibSass.Compiler.Options;

namespace SquishIt.Sass
{
    public class SassCompiler
    {
        public enum SassMode
        {
            Sass,
            Scss
        }

        public string CompileSass(string input, SassMode mode, string location, int precision = 5)
        {
            var compiler = new LibSass.Compiler.SassCompiler(new SassOptions
            {
                InputPath = location,
                Data = input,
                OutputStyle = SassOutputStyle.Nested,
                IncludeSourceComments = false,
                Precision = precision,
                IsIndentedSyntax = mode == SassMode.Sass
            });

            var result = compiler.Compile();

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new InvalidOperationException(string.Format("Sass compilation failed ({0})", result.ErrorMessage));
            }

            return result.Output;
        }
    }
}
