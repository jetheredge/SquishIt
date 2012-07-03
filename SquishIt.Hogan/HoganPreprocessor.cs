using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework;
using SquishIt.Hogan.Hogan;

namespace SquishIt.Hogan
{
    public class HoganPreprocessor : IPreprocessor
    {
        private const string validExtension = ".html";

        public bool ValidFor(string extension)
        {
            string upperExtension = extension.ToUpper();
            return Extensions.Contains(upperExtension.StartsWith(".") ? upperExtension : ("." + upperExtension));
        }

        public string Process(string filePath, string content)
        {
            var compiler = new HoganCompiler();
            string template = compiler.Compile(content);
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            var sb = new StringBuilder();
            sb.AppendLine("var JST = JST || {};");
            sb.AppendLine("JST['" + templateName + "'] = new Hogan.Template(" + template + ");");
            return sb.ToString();
        }

        public string[] Extensions
        {
            get { return new[] {validExtension.ToUpper()}; }
        }
    }
}