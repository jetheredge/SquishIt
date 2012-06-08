using System.IO;
using System.Security.Cryptography;
using System.Text;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Utilities
{
    public class Hasher : IHasher
    {
        protected IRetryableFileOpener RetryableFileOpener;
        public Hasher(IRetryableFileOpener retryableFileOpener)
        {
            RetryableFileOpener = retryableFileOpener;
        }

        /// <summary>
        /// Calculates an MD5 hash for a stream specified by <paramref name="stream" />
        /// </summary>
        /// <param name="stream">A <see cref="Stream" /> object specifying the file containing the content for which we're calculating the hash.</param>
        /// <returns>A string containing an MD5 hash.</returns>
        public string GetHash(Stream stream)
        {
            // Now that we have a byte array we can ask the CSP to hash it
            var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(stream);
            return ByteArrayToString(hashBytes);
        }

        /// <summary>
        /// Calculates an MD5 hash for a specified byte range of the file specified by <paramref name="fileInfo" />
        /// </summary>
        /// <param name="fileInfo">A <see cref="FileInfo" /> object specifying the file containing the content for which we're calculating the hash.</param>
        /// <returns>A string containing an MD5 hash.</returns>
        public string GetHash(FileInfo fileInfo)
        {
            using (var stream = RetryableFileOpener.OpenFileStream(fileInfo, 5, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Now that we have a byte array we can ask the CSP to hash it
                var md5 = new MD5CryptoServiceProvider();
                var hashBytes = md5.ComputeHash(stream);
                return ByteArrayToString(hashBytes);
            }
        }

        /// <summary>
        /// Calculates an MD5 hash for text specified by <paramref name="content" />
        /// </summary>
        /// <param name="content">A string containing the content for which we're calculating the hash.</param>
        /// <returns>A string containing an MD5 hash.</returns>
        public string GetHash(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(bytes);
            return ByteArrayToString(hashBytes);
        }

        static string ByteArrayToString(byte[] arrInput)
        {
            var output = new StringBuilder(arrInput.Length);
            for (var i = 0; i < arrInput.Length; i++)
            {
                output.Append(arrInput[i].ToString("X2"));
            }
            return output.ToString();
        }
    }
}