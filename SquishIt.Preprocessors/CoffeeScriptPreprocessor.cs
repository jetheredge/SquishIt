using System.Text.RegularExpressions;
using SquishIt.Framework;

namespace SquishIt.Preprocessors 
{
    public class CoffeeScriptPreprocessor : IPreprocessor 
    {
        static Regex lessFiles = new Regex(@"(\.coffee)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool ValidFor(string filePath) 
        {
            return lessFiles.IsMatch(filePath);
        }

        public string Process(string filePath, string content) 
        {
            var compiler = new Coffee.CoffeescriptCompiler();
            return compiler.Compile(content);
        }
    }
}
