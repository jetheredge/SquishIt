using System;
using System.Linq;
using System.Text.RegularExpressions;
using SquishIt.Framework;
using dotless.Core;

namespace SquishIt.Less
{
    public class LessPreprocessor : IPreprocessor
    {
        private static string[] extensions = new string[] { ".less", ".less.css" };
        static Regex lessFiles = new Regex(string.Format(@"(\{0})|(\{1})$", extensions), RegexOptions.Compiled);

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

        public string[] Extensions
        {
            get { return extensions.Select(ext => ext.ToUpper()).ToArray(); }
        }
    }
}
