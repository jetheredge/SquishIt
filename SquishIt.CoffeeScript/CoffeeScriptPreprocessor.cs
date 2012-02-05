using System;
using System.Text.RegularExpressions;
using SquishIt.CoffeeScript.Coffee;
using SquishIt.Framework;

namespace SquishIt.CoffeeScript 
{
    public class CoffeeScriptPreprocessor : IPreprocessor
    {
        static string extension = ".coffee";
        static Regex coffeeFiles = new Regex(@"(\." + extension + ")$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool ValidFor(string filePath) 
        {
            return coffeeFiles.IsMatch(filePath);
        }

        public string Process(string filePath, string content) 
        {
            var compiler = new CoffeeScriptCompiler();
            return compiler.Compile(content);
        }

        public string[] Extensions
        {
            get { return new [] { extension.ToUpper() }; }
        }
    }
}
