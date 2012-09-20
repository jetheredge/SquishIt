using System;
using System.Threading;

namespace SquishIt.Framework.Files
{
    public class CurrentDirectoryWrapper : ICurrentDirectoryWrapper
    {
        private static object lockObject = new object();
        private string previousDirectory;

        public T UsingCurrentDirectory<T>(string directory, Func<T> innerFunction)
        {
            lock (lockObject)
            {
                try
                {
                    SetCurrentDirectory(directory);
                    return innerFunction();
                }
                finally
                {
                    Revert();
                }
            }
        }

        private void SetCurrentDirectory(string directory)
        {
            if (!String.IsNullOrEmpty(directory))
            {
                previousDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = directory;
            }
        }

        private void Revert()
        {
            if (previousDirectory != null)
            {
                Environment.CurrentDirectory = previousDirectory;
            }
        }

    }
}