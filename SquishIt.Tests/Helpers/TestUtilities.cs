using System;
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
            if (FileSystem.Unix)
            {
                path = driveLetter.Replace(path, @"/")
                    .Replace(@"\", @"/");
            }
            return path;
        }

        //On Linux, only resolving paths relative to file system if HttpContext is null
        //This is lazy, but I don't have a need for command line tool at present
        public static string PreparePathRelativeToWorkingDirectory(string windowsPath)
        {
            var path = windowsPath;
            if (FileSystem.Unix)
            {
                var extendedPath = PreparePath(path);
                path = Environment.CurrentDirectory + extendedPath; //combine won't work here for some reason?
            }
            return path;
        }

        public static string NormalizeLineEndings(string contents)
        {
            //hash is calculated differently w/ different newline characters
            //normalize windows -> unix bc it's easier
            return contents.Replace("\r\n", "\n");
        }
    }
}

