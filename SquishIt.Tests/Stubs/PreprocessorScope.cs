using System;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs 
{
    public class ScriptPreprocessorScope<T> : IDisposable where T : IPreprocessor 
    {
        public ScriptPreprocessorScope(T instance)
        {
            Bundle.RegisterScriptPreprocessor(instance);
        }
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
        public StylePreprocessorScope(T instance)
        {
            Bundle.RegisterStylePreprocessor(instance);
        }

        public StylePreprocessorScope() 
        {
            Bundle.RegisterStylePreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Bundle.ClearPreprocessors();
        }
    }

    public class GlobalPreprocessorScope<T> : IDisposable where T:IPreprocessor
    {
        public GlobalPreprocessorScope(T instance)
        {
            Bundle.RegisterGlobalPreprocessor(instance);
        }

        public GlobalPreprocessorScope() 
        {
            Bundle.RegisterGlobalPreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Bundle.ClearPreprocessors();
        }
    }
}
