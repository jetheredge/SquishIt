using SquishIt.Framework.Base;
using dotless.Core;

namespace SquishIt.Less
{
    public class LessPreprocessor : Preprocessor
    {
        public override string[] Extensions
        {
            get { return new[] { ".less" }; }
        }

        public override string Process(string filePath, string content)
        {
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            return engine.TransformToCss(content, filePath);
        }
    }
}
