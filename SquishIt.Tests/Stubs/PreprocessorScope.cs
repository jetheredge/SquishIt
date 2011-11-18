using System;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs 
{
    public class PreprocessorScope<T> : IDisposable where T:IPreprocessor {
        public PreprocessorScope()
        {
            Bundle.RegisterPreprocessor<T>();
        }
        public void Dispose()
        {
            Bundle.ClearPreprocessors();
        }
    }
}
