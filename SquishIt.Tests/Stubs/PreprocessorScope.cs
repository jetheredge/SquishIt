using System;
using System.Linq;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs 
{
    public class PreprocessorScope<T> : IDisposable where T : IPreprocessor {
        public PreprocessorScope()
        {
            Bundle.RegisterPreprocessor<T>(Activator.CreateInstance<T>());
        }
        public void Dispose()
        {
            Bundle.RemovePreprocessors(Bundle.Preprocessors.SelectMany(pp => pp.Extensions).ToArray());
        }
    }
}
