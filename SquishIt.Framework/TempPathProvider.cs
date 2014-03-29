using System.IO;

namespace SquishIt.Framework
{
    public class TempPathProvider : ITempPathProvider
    {
        public string ForFile()
        {
            return Path.GetTempPath() + Path.GetRandomFileName();
        }
    }
}