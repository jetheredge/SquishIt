using System;
using System.Threading;

namespace SquishIt.Framework.Utilities
{
    public class CriticalRenderingSection : IDisposable
    {
        // this feels a bit like IDisposable abuse but allows us to code BundleBase in a more mutex-agnostic fashion
        // probably acceptable alternative for try .. finally though
        IFilePathMutexProvider mutexProvider;
        protected IFilePathMutexProvider MutexProvider
        {
            get { return mutexProvider ?? (mutexProvider = FilePathMutexProvider.Instance); }
            set { mutexProvider = value; }
        }

        readonly Mutex mutex;
        public CriticalRenderingSection(string path)
        {
            if(TrustLevel.IsHighOrUnrestrictedTrust)
            {
                mutex = MutexProvider.GetMutexForPath(path);
                mutex.WaitOne();
            }
        }

        public void Dispose()
        {
            if(TrustLevel.IsHighOrUnrestrictedTrust)
            {
                mutex.ReleaseMutex();
            }
        }
    }
}