using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SquishIt.Framework.FileResolvers;
using SquishIt.Framework.Files;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework
{
    public abstract class BundleBase
    {
        protected IFileWriterFactory fileWriterFactory;
        protected IFileReaderFactory fileReaderFactory;
        protected IDebugStatusReader debugStatusReader;
        protected ICurrentDirectoryWrapper currentDirectoryWrapper;
        protected IHasher hasher;
        protected Dictionary<string, string> attributes = new Dictionary<string, string>();

        protected BundleBase(IFileWriterFactory fileWriterFactory, IFileReaderFactory fileReaderFactory, IDebugStatusReader debugStatusReader, ICurrentDirectoryWrapper currentDirectoryWrapper, IHasher hasher)
        {
            this.fileWriterFactory = fileWriterFactory;
            this.fileReaderFactory = fileReaderFactory;
            this.debugStatusReader = debugStatusReader;
            this.currentDirectoryWrapper = currentDirectoryWrapper;
            this.hasher = hasher;
        }

        protected string RenderFiles(string template, IEnumerable<string> files)
        {
            var sb = new StringBuilder();
            foreach (string file in files)
            {
                string processedFile = ExpandAppRelativePath(file);
                sb.Append(String.Format(template, processedFile));
            }
            return sb.ToString();
        }

        protected List<string> GetFiles(List<InputFile> fileArguments)
        {
            var files = new List<string>();
            var fileResolverCollection = new FileResolverCollection();
            foreach (InputFile file in fileArguments)
            {
                files.AddRange(fileResolverCollection.Resolve(file.FilePath, file.FileType));
            }
            return files;
        }

        protected void WriteGZippedFile(string outputJavaScript, string gzippedOutputFile)
        {
            if (gzippedOutputFile != null)
            {
                var gzipper = new FileGZipper();
                gzipper.Zip(gzippedOutputFile, outputJavaScript);
            }
        }

        protected List<InputFile> GetFilePaths(List<string> list)
        {
            var result = new List<InputFile>();
            foreach (string file in list)
            {
                string mappedPath = FileSystem.ResolveAppRelativePathToFileSystem(file);
                result.Add(new InputFile(mappedPath, FileResolver.Type));
            }
            return result;
        }

        protected List<InputFile> GetEmbeddedResourcePaths(List<string> list)
        {
            var result = new List<InputFile>();
            foreach (string file in list)
            {
                result.Add(new InputFile(file, EmbeddedResourceResolver.Type));
            }
            return result;
        }

        protected string ExpandAppRelativePath(string file)
        {            
            if (file.StartsWith("~/"))
            {
                string appRelativePath = HttpRuntime.AppDomainAppVirtualPath;
                if (appRelativePath != null && !appRelativePath.EndsWith("/"))
                    appRelativePath += "/";
                return file.Replace("~/", appRelativePath);    
            }
            return file;
        }

        protected string ReadFile(string file)
        {
            using (var sr = fileReaderFactory.GetFileReader(file))
            {
                return sr.ReadToEnd();
            }
        }

        protected bool FileExists(string file)
        {
            return fileReaderFactory.FileExists(file);
        }

        protected string GetAdditionalAttributes()
        {
            var result = new StringBuilder();
            foreach (string key in attributes.Keys)
            {
                result.Append(key);
                result.Append("=\"");
                result.Append(attributes[key]);
                result.Append("\" ");
            }
            return result.ToString();
        }
    }
}