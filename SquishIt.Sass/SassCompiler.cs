using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using IronRuby;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using SquishIt.Framework;

namespace SquishIt.Sass
{
    //much thanks here to Paul Betts' SassAndCoffee project (https://github.com/xpaulbettsx/SassAndCoffee)
    public class SassCompiler
    {
        internal static string RootAppPath;

        public SassCompiler(string rootPath)
        {
            RootAppPath = rootPath;
        }

        public string CompileSass(string input, string fileName)
        {
            SassMode sassMode = SassMode.Scss;
            if(Regex.IsMatch(fileName, @"\.sass$")) sassMode = SassMode.Sass;

            return CompileSass(input, sassMode);
        }

        public enum SassMode
        {
            Sass,
            Scss
        }

        public string CompileSass(string input, SassMode mode)
        {
            var srs = new ScriptRuntimeSetup() { HostType = typeof(ResourceAwareScriptHost) };
            srs.AddRubySetup();
            var runtime = Ruby.CreateRuntime(srs);

            var engine = runtime.GetRubyEngine();

            engine.SetSearchPaths(new List<string> { @"R:\lib\ironruby", @"R:\lib\ruby\1.9.1" });

            var source = engine.CreateScriptSourceFromString(Utility.ResourceAsString("SquishIt.Sass.lib.sass_in_one.rb"), SourceCodeKind.File);
            var scope = engine.CreateScope();
            source.Execute(scope);

            dynamic sassMode = mode == SassMode.Sass
                                   ? engine.Execute("{:syntax => :sass}")
                                   : engine.Execute("{:syntax => :scss}");

            var sassEngine = scope.Engine.Runtime.Globals.GetVariable("Sass");

            return (string) sassEngine.compile(input, sassMode);
        }
    }

    public class ResourceAwareScriptHost : ScriptHost
    {
        PlatformAdaptationLayer _innerPal = null;
        public override PlatformAdaptationLayer PlatformAdaptationLayer
        {
            get { return _innerPal ?? (_innerPal = new ResourceAwarePAL()); }
        }
    }

    public class ResourceAwarePAL : PlatformAdaptationLayer
    {
        public override Stream OpenInputFileStream(string path)
        {
            var ret = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathToResourceName(path));
            if (ret != null)
            {
                return ret;
            }

            if (SassCompiler.RootAppPath == null || !path.ToLowerInvariant().StartsWith(SassCompiler.RootAppPath))
            {
                return null;
            }

            return base.OpenInputFileStream(path);
        }

        public override bool FileExists(string path)
        {
            if (Assembly.GetExecutingAssembly().GetManifestResourceInfo(pathToResourceName(path)) != null)
            {
                return true;
            }

            return base.FileExists(path);
        }

        string pathToResourceName(string path)
        {
            path = Platform.Mono ?
                path.Replace(Environment.CurrentDirectory, string.Empty).TrimStart(new [] { '/' }):
                path.Replace("1.9.1", "_1._9._1");
            
            return path   
                .Replace('\\', '.')
                .Replace('/', '.')
                .Replace("R:", "SquishIt.Sass"); // TODO: CHANGE APP NAMESPACE!!
        }
    }
}
