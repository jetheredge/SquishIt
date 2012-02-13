using System;
using System.Linq;
using System.Text.RegularExpressions;
using SquishIt.CoffeeScript.Coffee;
using SquishIt.Framework;

namespace SquishIt.CoffeeScript 
{
    public class CoffeeScriptPreprocessor : IPreprocessor
    {
        static string validExtension = ".coffee";
        //static Regex coffeeFiles = new Regex(@"(\." + extension + ")$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool ValidFor(string extension) 
        {
            var upperExtension = extension.ToUpper();
            return Extensions.Contains(upperExtension.StartsWith(".") ? upperExtension : ("." + upperExtension));
        }

        public string Process(string filePath, string content) 
        {
            var compiler = new CoffeeScriptCompiler();
            return compiler.Compile(content);
        }

        public string[] Extensions
        {
            get { return new [] { validExtension.ToUpper() }; }
        }
    }
}
