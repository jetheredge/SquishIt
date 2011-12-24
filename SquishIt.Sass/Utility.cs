using System.IO;
using System.Reflection;
using System.Text;

namespace SquishIt.Sass
{
    public class Utility
    {
        public static string ResourceAsString(string resource, Assembly assembly = null, Encoding enc = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();
            enc = enc ?? Encoding.UTF8;

            using (var ms = new MemoryStream())
            {
                assembly.GetManifestResourceStream(resource).CopyTo(ms);
                return enc.GetString(ms.GetBuffer()).Replace('\0', ' ').Trim();
            }
        }
    }
}
