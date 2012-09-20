using System;

namespace SquishIt.Framework.Files
{
    public class DirectoryWrapper : IDirectoryWrapper
    {
        private readonly static object lockObject = new object();
        private string previousDirectory;

        public T ExecuteInDirectory<T>(string directory, Func<T> innerFunction)
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