using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace JavascriptBundler.Minifiers
{
    public class ClosureMinifier: IFileCompressor
    {
        public string Compress(string file)
        {
            var a = Assembly.GetEntryAssembly();
            string path = Path.GetDirectoryName(a.Location);
            string outFile = Path.GetTempPath() + Path.GetRandomFileName();
            try
            {                
                string arguments = "-jar \"{0}\\closure-compiler\\compiler.jar\" --js \"{1}\" --js_output_file \"{2}\"";
                arguments = String.Format(arguments, path, file, outFile);
                var startInfo = new ProcessStartInfo("java", arguments);
                startInfo.UseShellExecute = false;
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardError = true;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                var process = Process.Start(startInfo);
                Console.Error.Write(process.StandardError.ReadToEnd());                    
                process.WaitForExit();                

                using (var sr = new StreamReader(outFile))
                {
                    return sr.ReadToEnd();
                }
            }
            finally
            {
                File.Delete(outFile);
            }            
        }
    }
}