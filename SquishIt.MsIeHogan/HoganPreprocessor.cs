using System.IO;
using System.Linq;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework;
using SquishIt.MsIeHogan.Hogan;

namespace SquishIt.MsIeHogan
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
            string renderFunc = compiler.Compile(content);
            string templateName = Path.GetFileName(filePath).Split('.').First();
            var sb = new StringBuilder();
            sb.AppendLine("var JST = JST || {};");
            sb.AppendLine("JST['" + templateName + "'] = new Hogan.Template(" + renderFunc + ",\"" + content.Replace("\"", "\\\"") + "\",Hogan,{});");
            return new ProcessResult(sb.ToString());
        }
    }
}