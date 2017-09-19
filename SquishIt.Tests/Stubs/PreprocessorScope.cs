using System;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs 
{
    public class ScriptPreprocessorScope<T> : IDisposable where T : IPreprocessor 
    {
        public ScriptPreprocessorScope(T instance)
        {
            Configuration.Instance.RegisterScriptPreprocessor(instance);
        }
        public ScriptPreprocessorScope() 
        {
            Configuration.Instance.RegisterScriptPreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Configuration.Instance.ClearPreprocessors();
        }
    }

    public class StylePreprocessorScope<T> : IDisposable where T : IPreprocessor 
    {
        public StylePreprocessorScope(T instance)
        {
            Configuration.Instance.RegisterStylePreprocessor(instance);
        }

        public StylePreprocessorScope() 
        {
            Configuration.Instance.RegisterStylePreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Configuration.Instance.ClearPreprocessors();
        }
    }

    public class GlobalPreprocessorScope<T> : IDisposable where T:IPreprocessor
    {
        public GlobalPreprocessorScope(T instance)
        {
            Configuration.Instance.RegisterGlobalPreprocessor(instance);
        }

        public GlobalPreprocessorScope() 
        {
            Configuration.Instance.RegisterGlobalPreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose() 
        {
            Configuration.Instance.ClearPreprocessors();
        }
    }
}
