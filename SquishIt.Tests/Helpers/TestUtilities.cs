using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SquishIt.Framework;

namespace SquishIt.Tests.Helpers
{
    public class TestUtilities
    {
        static readonly Regex driveLetter = new Regex(@"[a-zA-Z]{1}:\\");

        public static string PreparePath(string windowsPath)
        {
            var path = windowsPath;
            if (Platform.Unix)
            {
                path = driveLetter.Replace(path, @"/")
                    .Replace(@"\", @"/");
            }
            return path;
        }

        public static string PrepareRelativePath(string path)
        {
            var directorySeparator = Path.DirectorySeparatorChar.ToString();
            return Environment.CurrentDirectory + (path.StartsWith(directorySeparator) ? "" : directorySeparator) + PreparePath(path);
        }

        public static string NormalizeLineEndings(string contents)
        {
            //hash is calculated differently w/ different newline characters
            //normalize windows -> unix bc it's easier
            return contents.Replace("\r\n", "\n");
        }

        public static string CreateFile(string path, string contents)
        {
            (new FileInfo(path)).Directory.Create();
            using (var file = File.Create(path))
            {
                var bytes = Encoding.UTF8.GetBytes(contents);
                file.Write(bytes, 0, bytes.Length);
            }
            return path;
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}

