using System.IO;
using System.IO.Compression;

namespace JavascriptBundler
{
    public class FileGZipper
    {
        public void Zip(string file, string fileContents)
        {
            using (FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(new GZipStream(fileStream, CompressionMode.Compress)))
                {
                    writer.Write(fileContents);
                }
            }                        
        }
    }
}