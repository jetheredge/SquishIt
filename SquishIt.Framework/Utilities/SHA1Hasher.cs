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
            //forcing the issue here to avoid SHA1.Managed being chosen as default (not FIPS-compliant)
            return SHA1Cng.Create();
        }
    }
}