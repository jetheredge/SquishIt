﻿using System;
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
            string renderFunc = compiler.Compile(content);
            string templateName = Path.GetFileName(filePath).Split('.').First();
		    string templateHtml = string.Join("\\r\"+\"\\n",
		                           content.Replace("\"", "\\\"").Split(new[] {Environment.NewLine}, StringSplitOptions.None));
            var sb = new StringBuilder();
            sb.AppendLine("var JST = JST || {};");
            sb.AppendLine("JST['" + templateName + "'] = new Hogan.Template(" + renderFunc + ",\"" + templateHtml + "\",Hogan,{});");
            return new ProcessResult(sb.ToString());
        }
    }
}