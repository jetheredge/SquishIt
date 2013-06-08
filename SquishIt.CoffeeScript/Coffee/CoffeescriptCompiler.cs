using System;
using System.IO;
using System.Reflection;
using Jurassic;
using SquishIt.Framework;

namespace SquishIt.CoffeeScript.Coffee
{
    public class CoffeeScriptCompiler
    {
        static string _coffeescript;
        static ScriptEngine _engine;

        public string Compile(string input)
        {
            if(Platform.Mono)
            {
                throw new NotSupportedException("Coffeescript not yet supported for mono.");
            }

            CoffeeScriptEngine.SetGlobalValue("Source", input);

            // Errors go from here straight on to the rendered page; 
            // we don't want to hide them because they provide valuable feedback
            // on the location of the error
            var result = CoffeeScriptEngine.Evaluate<string>("CoffeeScript.compile(Source, {bare: false})");

            return result;
        }

        static ScriptEngine CoffeeScriptEngine
        {
            get
            {
                if(_engine == null)
                {
                    lock(typeof(CoffeeScriptCompiler))
                    {
                        var engine = new ScriptEngine {ForceStrictMode = true};
                        engine.Execute(Compiler);
                        _engine = engine;
                    }
                }
                return _engine;
            }
        }

        static string Compiler
        {
            get
            {
                if(_coffeescript == null)
                    _coffeescript = LoadCoffeescript();

                return _coffeescript;
            }
        }

        static string LoadCoffeescript()
        {
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SquishIt.CoffeeScript.Coffee.coffee-script.js"))
            {
                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
