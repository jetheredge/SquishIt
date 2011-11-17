using System.Text.RegularExpressions;
using SquishIt.Framework;
using dotless.Core;

namespace SquishIt.Less
{
    public class LessPreprocessor : IPreprocessor
    {
        static Regex lessFiles = new Regex(@"(\.less)|(\.less.css)$", RegexOptions.Compiled);


        public bool ValidFor(string filePath)
        {
            return lessFiles.IsMatch(filePath);
        }

        public string Process(string filePath, string content)
        {
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            return engine.TransformToCss(content, filePath);
        }
    }
}
