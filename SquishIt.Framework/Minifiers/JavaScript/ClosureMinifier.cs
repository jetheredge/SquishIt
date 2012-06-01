using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SquishIt.Framework.Minifiers.JavaScript
{
    public class ClosureMinifier: IJavaScriptMinifier
    {
        string CompressFile(string file)
        {
            string path;
            if (HttpContext.Current != null)
            {
                path = HttpContext.Current.Server.MapPath("~/bin");
            }
            else
            {
                var a = Assembly.GetExecutingAssembly();
                path = Path.GetDirectoryName(a.Location);
            }
            
            string outFile = Path.GetTempPath() + Path.GetRandomFileName();
            try
            {                
                string arguments = "-jar \"{0}\\closure-compiler\\compiler.jar\" --js \"{1}\" --js_output_file \"{2}\"";
                arguments = String.Format(arguments, path, file, outFile);
                var startInfo = new ProcessStartInfo("java", arguments);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardError = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                var process = Process.Start(startInfo);
                process.WaitForExit();

                string output;
                using (var sr = new StreamReader(outFile))
                {
                    output = sr.ReadToEnd();
                }
                return output;
            }
            finally
            {
                File.Delete(outFile);
            }            
        }

        public string Minify(string content)
        {
            string inputFileName = Path.GetTempPath() + Path.GetRandomFileName();
            try
            {
                using (var sw = new StreamWriter(inputFileName))
                {
                    sw.Write(content);
                }
                return CompressFile(inputFileName);
            }
            finally
            {
                File.Delete(inputFileName);
            }
        }
    }
}