using System.Collections.Generic;
using System.Threading;

namespace SquishIt.Framework.Renderers
{
    public class CacheRenderer: IRenderer
    {
        private readonly string prefix;
        private readonly string name;
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        private static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        public CacheRenderer(string prefix, string name)
        {
            this.prefix = prefix;
            this.name = name;
        }

        public void Render(string content, string outputFile)
        {
            readerWriterLockSlim.EnterWriteLock();
            try
            {
                cache[prefix + "_" + name] = content;
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        public static string Get(string prefix, string name)
        {
            readerWriterLockSlim.EnterReadLock();
            try
            {
                return cache[prefix + "_" + name];
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }
    }
}