using NSass;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace SquishIt.NSass
{
    public class NSassPreprocessor : Preprocessor
    {
        public override IProcessResult Process(string filePath, string content)
        {
            return new ProcessResult(new SassCompiler().Compile(content));
        }
        public override string[] Extensions
        {
            get { return new[] { ".scss" }; }
        }
    }
}