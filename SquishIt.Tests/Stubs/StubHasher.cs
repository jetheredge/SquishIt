using System.IO;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Stubs
{
    public class StubHasher : IHasher
    {
        protected string HashValue;
        public StubHasher(string hashValue)
        {
            HashValue = hashValue;
        }

        public string GetHash(Stream stream)
        {
            return HashValue;
        }

        public string GetHash(FileInfo fileInfo)
        {
            return HashValue;
        }

        public string GetHash(string content)
        {
            return HashValue;
        }
    }
}
