using System.Security.Cryptography;
using System.Text;

namespace SquishIt.Framework.Utilities
{
    public class Hasher
    {
        public static string Create(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            byte[] hashBytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            return ByteArrayToString(hashBytes);
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            var output = new StringBuilder(arrInput.Length);
            for (int i = 0; i < arrInput.Length; i++)
            {
                output.Append(arrInput[i].ToString("X2"));
            }
            return output.ToString();
        }
    }
}