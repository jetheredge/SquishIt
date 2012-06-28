using System.IO;
using System.Reflection;
using Jurassic;

namespace SquishIt.Hogan.Hogan
{
    public class HoganCompiler
    {
        private static string _hogan;
        private readonly ScriptEngine _scriptEngine;

        public HoganCompiler()
        {
            _scriptEngine = new ScriptEngine {EnableDebugging = true};
            _scriptEngine.Execute(Compiler);
            _scriptEngine
                .Evaluate("var compile = function (template) {return Hogan.compile(template, { asString: 1 });};");
        }

        private static string Compiler
        {
            get { return _hogan ?? (_hogan = LoadHogan()); }
        }

        public string Compile(string input)
        {
            return _scriptEngine.CallGlobalFunction<string>("compile", input);
        }

        private static string LoadHogan()
        {
            using (Stream stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("SquishIt.Hogan.Hogan.hogan-2.0.0.js"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}