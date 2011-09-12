using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EcmaScript.NET;
using Jurassic;

namespace SquishIt.Framework.Coffee
{
    public class CoffeescriptCompiler
    {
        private static string _coffeescript;
        private static ScriptEngine _engine;

        public string Compile(string input)
        {
            CoffeeScriptEngine.SetGlobalValue("Source", input);

            // Errors go from here straight on to the rendered page; 
            // we don't want to hide them because they provide valuable feedback
            // on the location of the error
            string result = CoffeeScriptEngine.Evaluate<string>("CoffeeScript.compile(Source, {bare: true})");

            return result;
        }

        private static ScriptEngine CoffeeScriptEngine
        {
            get
            {
                if(_engine == null)
                {
                    var engine = new ScriptEngine();
                    engine.ForceStrictMode = true;
                    engine.Execute(Compiler);
                    _engine = engine;
                }
                return _engine;
            }
        }

        public static string Compiler 
        { 
            get
            {
                if (_coffeescript == null)
                    _coffeescript = LoadCoffeescript();

                return _coffeescript;
            }
        }

        private static string LoadCoffeescript()
        {
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SquishIt.Framework.Coffee.coffee-script.js"))
            {
                using(var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
