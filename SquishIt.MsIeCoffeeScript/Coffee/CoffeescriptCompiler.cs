﻿using System;
using SquishIt.Framework;

using MsieJavaScriptEngine;
using MsieJavaScriptEngine.ActiveScript;

namespace SquishIt.MsIeCoffeeScript.Coffee
{
    public class CoffeeScriptCompiler : IDisposable
    {
        /// <summary>
        /// Name of resource, which contains a CoffeeScript-library
        /// </summary>
        const string COFFEESCRIPT_LIBRARY_RESOURCE_NAME
            = "SquishIt.MsIeCoffeeScript.Coffee.coffee-script.js";

        /// <summary>
        /// Name of function, which is responsible for CoffeeScript-compilation
        /// </summary>
        const string COMPILATION_FUNCTION_NAME = "coffeeScriptCompile";

        /// <summary>
        /// MSIE JS engine
        /// </summary>
        private MsieJsEngine _jsEngine;

        /// <summary>
        /// Synchronizer of compilation
        /// </summary>
        private readonly object _compilationSynchronizer = new object();

        /// <summary>
        /// Flag that compiler is initialized
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Initializes compiler
        /// </summary>
        private void Initialize()
        {
            if (!_initialized)
            {
                _jsEngine = new MsieJsEngine(true);
                _jsEngine.ExecuteResource(COFFEESCRIPT_LIBRARY_RESOURCE_NAME, GetType());
                _jsEngine.Execute(string.Format(@"var {0} = function(code) {{ 	return CoffeeScript.compile(code, {{ bare: false }});}}", COMPILATION_FUNCTION_NAME));

                _initialized = true;
            }
        }

        public string Compile(string input)
        {
            if(FileSystem.Unix)
            {
                throw new NotSupportedException("Coffeescript not yet supported for mono.");
            }

            string newContent;

            lock (_compilationSynchronizer)
            {
                Initialize();

                try
                {
                    newContent = _jsEngine.CallFunction<string>(COMPILATION_FUNCTION_NAME, input);
                }
                catch (ActiveScriptException e)
                {
                    throw new Exception(ActiveScriptErrorFormatter.Format(e));
                }
            }

            return newContent;
        }

        /// <summary>
        /// Flag that object is destroyed
        /// </summary>
        private bool _disposed;


        /// <summary>
        /// Destructs instance of CoffeeScript-compiler
        /// </summary>
        ~CoffeeScriptCompiler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Destroys object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destroys object
        /// </summary>
        /// <param name="disposing">Flag, allowing destruction of 
        /// managed objects contained in fields of class</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_jsEngine != null)
                {
                    _jsEngine.Dispose();
                }
            }
        }
    }
}
