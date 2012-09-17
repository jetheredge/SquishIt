﻿using SquishIt.CoffeeScript.Coffee;
using SquishIt.Framework.Base;
using SquishIt.Framework;

namespace SquishIt.CoffeeScript 
{
    public class CoffeeScriptPreprocessor : Preprocessor
    {
        public override string[] Extensions
        {
            get { return new [] { ".coffee" }; }
        }

		public override IProcessResult Process(string filePath, string content) 
        {
            var compiler = new CoffeeScriptCompiler();
			return new ProcessResult(compiler.Compile(content));
        }
    }
}
