using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Hogan.Hogan;
using SquishIt.Framework;

namespace SquishIt.Hogan
{
    public class HoganPreprocessor : Preprocessor
    {
        public override string[] Extensions
        {
            get { return new[] { ".hogan" }; }
        }

		public override IProcessResult Process(string filePath, string content)
        {
            var compiler = new HoganCompiler();
            string template = compiler.Compile(content);
            string templateName = Path.GetFileName(filePath).Split('.').First();
            var sb = new StringBuilder();
            sb.AppendLine("var JST = JST || {};");
            sb.AppendLine("JST['" + templateName + "'] = new Hogan.Template(" + template + ");");
            return new ProcessResult(sb.ToString());
        }
    }
}