using System.Collections.Generic;
using System.IO;
using System.Text;
using SquishIt.Framework.Base;
using SquishIt.Framework.Files;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.JavaScript {
    public class JavaScriptBundle : BundleBase<JavaScriptBundle> 
    {
        private const string JS_TEMPLATE = "<script type=\"text/javascript\" {0}src=\"{1}\"></script>";
        bool typeless;

        private const string CACHE_PREFIX = "js";

        protected override IMinifier<JavaScriptBundle> DefaultMinifier 
        {
            get { return new MsMinifier (); }
        }

        public JavaScriptBundle ()
            : base (new FileWriterFactory (new RetryableFileOpener (), 5), new FileReaderFactory (new RetryableFileOpener (), 5), new DebugStatusReader (), new CurrentDirectoryWrapper (), new Hasher (new RetryableFileOpener ()), new BundleCache ()) {}

        public JavaScriptBundle (IDebugStatusReader debugStatusReader)
            : base (new FileWriterFactory (new RetryableFileOpener (), 5), new FileReaderFactory (new RetryableFileOpener (), 5), debugStatusReader, new CurrentDirectoryWrapper (), new Hasher (new RetryableFileOpener ()), new BundleCache ()) {}

        public JavaScriptBundle (IDebugStatusReader debugStatusReader, IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher, IBundleCache bundleCache) :
            base (fileWriterFactory, fileReaderFactory, debugStatusReader, currentDirectoryWrapper, hasher, bundleCache) {}

        public JavaScriptBundle WithoutTypeAttribute () 
        {
            this.typeless = true;
            return this;
        }

        protected override string Template 
        {
            get { return typeless ? JS_TEMPLATE.Replace ("type=\"text/javascript\" ", "") : JS_TEMPLATE; }
        }

        protected override string CachePrefix 
        {
            get { return CACHE_PREFIX; }
        }

        internal override Dictionary<string, GroupBundle> BeforeRenderDebug()
        {
            var modifiedGroupBundles = new Dictionary<string, GroupBundle>(GroupBundles);

            foreach (var groupBundleKVP in modifiedGroupBundles)
            {
                var groupBundle = groupBundleKVP.Value;

                foreach (var asset in groupBundle.Assets)
                {
                    var localPath = asset.LocalPath;
                    if (localPath.ToLower().EndsWith(".coffee"))
                    {
                        string outputFile = FileSystem.ResolveAppRelativePathToFileSystem(localPath);
                        string javascript = ProcessCoffee(outputFile);
                        outputFile += ".debug.js";
                        using (var fileWriter = fileWriterFactory.GetFileWriter(outputFile))
                        {
                            fileWriter.Write(javascript);
                        }

                        asset.LocalPath = localPath + ".debug.js";
                    }
                }
            }

            return modifiedGroupBundles;
        }

        protected override string BeforeMinify(string outputFile, List<string> files)
        {
            var sb = new StringBuilder();
            foreach (var file in files)
            {
                string content = file.EndsWith(".coffee")
                                     ? ProcessCoffee(file)
                                     : ReadFile(file);

                sb.Append(content + "\n");
            }

            return sb.ToString();
        }

        private string ProcessCoffee(string file)
        {
            lock (typeof(JavaScriptBundle))
            {
                try
                {
                    currentDirectoryWrapper.SetCurrentDirectory(Path.GetDirectoryName(file));
                    var content = ReadFile(file);
                    var compiler = new Coffee.CoffeescriptCompiler();
                    return compiler.Compile(content);
                }
                finally
                {
                    currentDirectoryWrapper.Revert();
                }
            }
        }
    }
}