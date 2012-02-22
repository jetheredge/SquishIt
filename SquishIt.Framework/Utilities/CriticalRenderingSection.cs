using System;
using System.Threading;
using System.Web;
namespace SquishIt.Framework.Utilities 
{
    public class CriticalRenderingSection : IDisposable {
        // this feels a bit like IDisposable abuse but allows us to code BundleBase in a more mutex-agnostic fashion
        // probably acceptable alternative for try .. finally though
        private IFilePathMutexProvider mutexProvider;
        protected IFilePathMutexProvider MutexProvider 
        {
            get { return mutexProvider ?? (mutexProvider = FilePathMutexProvider.Instance); }
            set { mutexProvider = value; }
        }

        Mutex mutex;
        public CriticalRenderingSection(string path) 
        {
            if (IsHighOrUnrestrictedTrust) 
            {
                mutex = MutexProvider.GetMutexForPath(path);
                mutex.WaitOne();
            }
        }

        public void Dispose() 
        {
            if (IsHighOrUnrestrictedTrust) 
            {
                mutex.ReleaseMutex();
            }
        }

        bool IsHighOrUnrestrictedTrust 
        {
            get { return trustLevel == AspNetHostingPermissionLevel.High || trustLevel == AspNetHostingPermissionLevel.Unrestricted; }
        }

        AspNetHostingPermissionLevel trustLevel = GetCurrentTrustLevel();

        static AspNetHostingPermissionLevel GetCurrentTrustLevel() 
        {
            var lastTrustedLevel = AspNetHostingPermissionLevel.None;

            foreach (AspNetHostingPermissionLevel level in new[] {
                AspNetHostingPermissionLevel.Minimal,
                AspNetHostingPermissionLevel.Low,
                AspNetHostingPermissionLevel.Medium,
                AspNetHostingPermissionLevel.High,
                AspNetHostingPermissionLevel.Unrestricted }) 
            {
                try {
                    new AspNetHostingPermission(level).Demand();
                    lastTrustedLevel = level;
                }
                catch (System.Security.SecurityException) {
                    break;
                }
            }

            return lastTrustedLevel;
        }
    }
}