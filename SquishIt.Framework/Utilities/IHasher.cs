using System.IO;

namespace SquishIt.Framework.Utilities
{
    public interface IHasher
    {
        string GetHash(Stream stream);
        string GetHash(FileInfo fileInfo);
        string GetHash(string content);
    }
}
