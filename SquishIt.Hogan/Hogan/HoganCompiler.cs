using System;
using System.IO;
using System.Reflection;
using Jurassic;
using SquishIt.Framework;

namespace SquishIt.Hogan.Hogan
{
    public class HoganCompiler
    {
        static string _hogan;
        static ScriptEngine _engine;

        public string Compile(string input)
        {
            if(Platform.Mono)
            {
                throw new NotSupportedException("Hogan not yet supported for mono.");
            }

            return HoganEngine.CallGlobalFunction<string>("compile", input);
        }

        static ScriptEngine HoganEngine
        {
            get
            {
                if(_engine == null)
                {
                    lock(typeof(HoganCompiler))
                    {
                        var engine = new ScriptEngine { EnableDebugging = true };
                        engine.Execute(Compiler);
                        engine.Evaluate("var compile = function (template) {return Hogan.compile(template, { asString: 1 });};");
                        _engine = engine;
                    }
                }
                return _engine;
            }
        }

        static string Compiler
        {
            get { return _hogan ?? (_hogan = LoadHogan()); }
        }

        static string LoadHogan()
        {
            using(var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("SquishIt.Hogan.Hogan.hogan-2.0.0.js"))
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}