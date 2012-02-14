using System;
using System.Linq;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs 
{
    public class ScriptPreprocessorScope<T> : IDisposable where T : IPreprocessor 
    {
        public ScriptPreprocessorScope() 
        {
            Bundle.RegisterScriptPreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Bundle.ClearPreprocessors();
        }
    }

    public class StylePreprocessorScope<T> : IDisposable where T : IPreprocessor 
    {
        public StylePreprocessorScope() 
        {
            Bundle.RegisterStylePreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Bundle.ClearPreprocessors();
        }
    }
}
