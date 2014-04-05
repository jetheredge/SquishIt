using System.Security.Cryptography;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Utilities
{
    public class SHA1Hasher : Hasher
    {
        public SHA1Hasher(IRetryableFileOpener retryableFileOpener) : base(retryableFileOpener)
        {
        }

        protected override HashAlgorithm GetImplementation()
        {
            return SHA1.Create();
        }
    }
}