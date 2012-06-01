using System;

namespace SquishIt.Framework.Files
{
    public class CurrentDirectoryWrapper: ICurrentDirectoryWrapper
    {
        string previousDirectory;

        public void SetCurrentDirectory(string directory)
        {
            if (!String.IsNullOrEmpty(directory))
            {
                previousDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = directory;    
            }
        }

        public void Revert()
        {
            if (previousDirectory != null)
            {
                Environment.CurrentDirectory = previousDirectory;    
            }
        }
    }
}