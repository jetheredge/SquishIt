using System.Linq;
using SquishIt.Framework;
using dotless.Core;

namespace SquishIt.Less
{
    public class LessPreprocessor : IPreprocessor
    {
        static readonly string[] extensions = new[] { ".less", ".less.css" };
        //static Regex lessFiles = new Regex(string.Format(@"(\{0})|(\{1})$", extensions), RegexOptions.Compiled);

        public bool ValidFor(string extension)
        {
            var upperExtension = extension.ToUpper();
            return Extensions.Contains(upperExtension.StartsWith(".") ? upperExtension : ("." + upperExtension));
        }

        public string Process(string filePath, string content)
        {
            var engineFactory = new EngineFactory();
            var engine = engineFactory.GetEngine();
            return engine.TransformToCss(content, filePath);
        }

        public string[] Extensions
        {
            get { return extensions.Select(ext => ext.ToUpper()).ToArray(); }
        }
    }
}
