using System;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Helpers
{
    class FilePathMutexProviderScope : IDisposable
    {
        public FilePathMutexProviderScope(IFilePathMutexProvider instance)
        {
            FilePathMutexProvider.instance = instance;
        }
        public void Dispose()
        {
            FilePathMutexProvider.instance = null;
        }
    }
}
