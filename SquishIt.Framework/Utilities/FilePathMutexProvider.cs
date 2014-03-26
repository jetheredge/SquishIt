using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace SquishIt.Framework.Utilities
{
    public class FilePathMutexProvider : IFilePathMutexProvider
    {
        const string NullPathSurrogate = "<NULL>";

        internal static IFilePathMutexProvider instance;
        static readonly object createMutexLock = new object();

        readonly Dictionary<string, Mutex> pathMutexes =
            new Dictionary<string, Mutex>(StringComparer.Ordinal);
        readonly IHasher hasher;
        readonly IPathTranslator pathTranslator;

        public static IFilePathMutexProvider Instance
        {
            get { return instance ?? (instance = new FilePathMutexProvider(Configuration.Instance.DefaultHasher(), Configuration.Instance.DefaultPathTranslator())); }
        }

        public FilePathMutexProvider(IHasher hasher, IPathTranslator pathTranslator)
        {
            this.hasher = hasher;
            this.pathTranslator = pathTranslator;
        }

        public Mutex GetMutexForPath(string path)
        {
            Mutex result;

            string normalizedPath = GetNormalizedPath(path);
            if(pathMutexes.TryGetValue(normalizedPath, out result))
            {
                return result;
            }

            lock(createMutexLock)
            {
                if(pathMutexes.TryGetValue(normalizedPath, out result))
                {
                    return result;
                }

                result = CreateSharableMutexForPath(normalizedPath);
                pathMutexes[normalizedPath] = result;
            }

            return result;
        }

        string GetNormalizedPath(string path)
        {
            if(String.IsNullOrEmpty(path))
            {
                return NullPathSurrogate;
            }

            // Normalize the path
            var fileSystemPath = pathTranslator.ResolveAppRelativePathToFileSystem(path);
            // The path is lower cased to avoid different hashes. Even on a case sensitive
            // file system this probably is okay, since it's a web application
            return Path.GetFullPath(fileSystemPath)
                .ToLowerInvariant();
        }

        Mutex CreateSharableMutexForPath(string normalizedPath)
        {
            // The path is transformed to a hash value to avoid getting an invalid Mutex name.
            var mutexName = @"Global\SquishitPath" + hasher.GetHash(normalizedPath);
            return CreateSharableMutex(mutexName);
        }

        static Mutex CreateSharableMutex(string name)
        {
            // Creates a mutex sharable by more than one process

            // The constructor will either create new mutex or open
            // an existing one, in a thread-safe manner
            bool createdNew;

            var everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            if(Platform.Mono)
            {
                //MutexAccessRules don't seem to work on mono yet: http://lists.ximian.com/pipermail/mono-list/2008-August/039294.html
                return new Mutex(false, name, out createdNew);
            }
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(new MutexAccessRule(everyoneSid, MutexRights.FullControl, AccessControlType.Allow));
            return new Mutex(false, name, out createdNew, mutexSecurity);
        }
    }
}
