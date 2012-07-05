using SquishIt.CoffeeScript.Coffee;
using SquishIt.Framework.Base;

namespace SquishIt.CoffeeScript 
{
    public class CoffeeScriptPreprocessor : Preprocessor
    {
        public override string[] Extensions
        {
            get { return new [] { ".coffee" }; }
        }

        public override string Process(string filePath, string content) 
        {
            var compiler = new CoffeeScriptCompiler();
            return compiler.Compile(content);
        }
    }
}
