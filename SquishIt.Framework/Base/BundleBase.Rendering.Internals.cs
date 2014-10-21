using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SquishIt.Framework.Files;
using SquishIt.Framework.Renderers;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Base
{
    public abstract partial class BundleBase<T> where T : BundleBase<T>
    {
        IEnumerable<string> GetFiles(IEnumerable<Asset> assets)
        {
            var inputFiles = assets.Select(a => Input.FromAsset(a, pathTranslator, IsDebuggingEnabled));
            var resolvedFilePaths = new List<string>();

            foreach (Input input in inputFiles)
            {
                resolvedFilePaths.AddRange(input.Resolve(allowedExtensions, disallowedExtensions, debugExtension));
            }

            return resolvedFilePaths;
        }

        protected IEnumerable<string> GetFilesForSingleAsset(Asset asset)
        {
            var inputFile = Input.FromAsset(asset, pathTranslator, IsDebuggingEnabled);
            return inputFile.Resolve(allowedExtensions, disallowedExtensions, debugExtension);
        }

        protected IPreprocessor[] FindPreprocessors(string file)
        {
            //using rails convention of applying preprocessing based on file extension components in reverse order
            return file.Split('.')
                .Skip(1)
                .Reverse()
                .Select(FindPreprocessor)
                .TakeWhile(p => p != null)
                .Where(p => !(p is NullPreprocessor))
                .ToArray();
        }

        protected string PreprocessFile(string file, IPreprocessor[] preprocessors)
        {
            return directoryWrapper.ExecuteInDirectory(Path.GetDirectoryName(file),
                () => PreprocessContent(file, preprocessors, ReadFile(file)));
        }

        protected string PreprocessArbitrary(Asset asset)
        {
            if (!asset.IsArbitrary) throw new InvalidOperationException("PreprocessArbitrary can only be called on Arbitrary assets.");
            
            var filename = "dummy." + (asset.Extension ?? defaultExtension);
            var preprocessors = FindPreprocessors(filename);
            return asset.ArbitraryWorkingDirectory != null ?
                        directoryWrapper.ExecuteInDirectory(asset.ArbitraryWorkingDirectory, () => MinifyIfNeeded(PreprocessContent(filename, preprocessors, asset.Content), asset.Minify)) :
                        MinifyIfNeeded(PreprocessContent(filename, preprocessors, asset.Content), asset.Minify);
        }

        protected string PreprocessContent(string file, IPreprocessor[] preprocessors, string content)
        {
            return preprocessors.NullSafeAny()
                       ? preprocessors.Aggregate(content, (cntnt, pp) =>
                                                              {
                                                                  var result = pp.Process(file, cntnt);
                                                                  bundleState.DependentFiles.AddRange(result.Dependencies);
                                                                  return result.Result;
                                                              })
                       : content;
        }

        IPreprocessor FindPreprocessor(string extension)
        {
            var instanceTypes = bundleState.Preprocessors.Select(ipp => ipp.GetType()).ToArray();

            var preprocessor = 
                bundleState.Preprocessors.Union(Bundle.Preprocessors.Where(pp => !instanceTypes.Contains(pp.GetType())))
                .FirstOrDefault(p => p.ValidFor(extension));

            return preprocessor;
        }

        string ExpandAppRelativePath(string file)
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

        bool FileExists(string file)
        {
            return fileReaderFactory.FileExists(file);
        }

        string GetAdditionalAttributes(BundleState state)
        {
            var result = new StringBuilder();
            foreach (var attribute in state.Attributes)
            {
                result.Append(attribute.Key);
                result.Append("=\"");
                result.Append(attribute.Value);
                result.Append("\" ");
            }
            return result.ToString();
        }

        string GetFilesForRemote(IEnumerable<string> remoteAssetPaths, BundleState state)
        {
            var sb = new StringBuilder();
            foreach (var uri in remoteAssetPaths)
            {
                sb.Append(FillTemplate(state, uri));
            }

            return sb.ToString();
        }

        string BuildAbsolutePath(string siteRelativePath)
        {
            if (HttpContext.Current == null)
                throw new InvalidOperationException(
                    "Absolute path can only be constructed in the presence of an HttpContext.");
            if (!siteRelativePath.StartsWith("/"))
                throw new InvalidOperationException("This helper method only works with site relative paths.");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;
            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port,
                                 VirtualPathUtility.ToAbsolute(siteRelativePath));
        }

        string Render(string renderTo, string key, IRenderer renderer)
        {
            var cacheUniquenessHash = key.Contains("#") ? hasher.GetHash(bundleState.Assets
                                               .Select(a => a.IsRemote ? a.RemotePath :
                                                   a.IsArbitrary ? a.Content : a.LocalPath)
                                               .OrderBy(s => s)
                                               .Aggregate(string.Empty, (acc, val) => acc + val)) : string.Empty;

            key = CachePrefix + key + cacheUniquenessHash;

            if (!String.IsNullOrEmpty(BaseOutputHref))
            {
                key = BaseOutputHref + key;
            }

            if (IsDebuggingEnabled())
            {
                var content = RenderDebug(renderTo, key, renderer);
                return content;
            }
            return RenderRelease(key, renderTo, renderer);
        }

        string RenderDebug(string renderTo, string name, IRenderer renderer)
        {
            bundleState.DependentFiles.Clear();

            var renderedFiles = new HashSet<string>();

            var sb = new StringBuilder();

            bundleState.DependentFiles.AddRange(GetFiles(bundleState.Assets.Where(a => !a.IsArbitrary).ToList()));
            foreach (var asset in bundleState.Assets)
            {
                if (asset.IsArbitrary)
                {
                    var filename = "dummy" + asset.Extension;
                    var preprocessors = FindPreprocessors(filename);
                    var processedContent = asset.ArbitraryWorkingDirectory != null ?
                           directoryWrapper.ExecuteInDirectory(asset.ArbitraryWorkingDirectory, () => PreprocessContent(filename, preprocessors, asset.Content)) :
                           PreprocessContent(filename, preprocessors, asset.Content);
                    sb.AppendLine(string.Format(tagFormat, processedContent));
                }
                else
                {
                    var inputFile = Input.FromAsset(asset, pathTranslator, IsDebuggingEnabled);
                    var files = inputFile.Resolve(allowedExtensions, disallowedExtensions, debugExtension);

                    if (asset.IsEmbeddedResource)
                    {
                        var tsb = new StringBuilder();

                        foreach (var fn in files)
                        {
                            tsb.Append(ReadFile(fn) + "\n\n\n");
                        }

                        var processedFile = ExpandAppRelativePath(asset.LocalPath);
                        //embedded resources need to be rendered regardless to be usable
                        renderer.Render(tsb.ToString(), pathTranslator.ResolveAppRelativePathToFileSystem(processedFile));
                        sb.AppendLine(FillTemplate(bundleState, processedFile));
                    }
                    else if (asset.RemotePath != null)
                    {
                        sb.AppendLine(FillTemplate(bundleState, ExpandAppRelativePath(asset.LocalPath)));
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            if (!renderedFiles.Contains(file))
                            {
                                var fileBase = pathTranslator.ResolveAppRelativePathToFileSystem(asset.LocalPath);
                                var newPath = PreprocessForDebugging(file).Replace(fileBase, "");
                                var path = ExpandAppRelativePath(asset.LocalPath + newPath.Replace("\\", "/"));
                                sb.AppendLine(FillTemplate(bundleState, path));
                                renderedFiles.Add(file);
                            }
                        }
                    }
                }
            }

            var content = sb.ToString();

            if (bundleCache.ContainsKey(name))
            {
                bundleCache.Remove(name);
            }
            if (debugStatusReader.IsDebuggingEnabled()) //default for predicate is null - only want to add to cache if not forced via predicate
            {
                bundleCache.Add(name, content, bundleState.DependentFiles, IsDebuggingEnabled());
            }

            //need to render the bundle to caches, otherwise leave it
            if (renderer is CacheRenderer)
                renderer.Render(content, renderTo);

            return content;
        }

        bool TryGetCachedBundle(string key, out string content)
        {
            if (bundleState.DebugPredicate.SafeExecute())
            {
                content = "";
                return false;
            }
            return bundleCache.TryGetValue(key, out content);
        }

        string RenderRelease(string key, string renderTo, IRenderer renderer)
        {
            string content;
            if (!TryGetCachedBundle(key, out content))
            {
                using (new CriticalRenderingSection(renderTo))
                {
                    if (!TryGetCachedBundle(key, out content))
                    {
                        var uniqueFiles = new List<string>();
                        string hash = null;

                        bundleState.DependentFiles.Clear();

                        if (renderTo == null)
                        {
                            renderTo = renderPathCache[CachePrefix + "." + key];
                        }
                        else
                        {
                            renderPathCache[CachePrefix + "." + key] = renderTo;
                        }

                        var outputFile = pathTranslator.ResolveAppRelativePathToFileSystem(renderTo);
                        var renderToPath = ExpandAppRelativePath(renderTo);

                        if (!String.IsNullOrEmpty(BaseOutputHref))
                        {
                            renderToPath = String.Concat(BaseOutputHref.TrimEnd('/'), "/", renderToPath.TrimStart('/'));
                        }

                        var remoteAssetPaths = new List<string>();
                        remoteAssetPaths.AddRange(bundleState.Assets.Where(a => a.IsRemote).Select(a => a.RemotePath));

                        uniqueFiles.AddRange(GetFiles(bundleState.Assets.Where(asset =>
                            asset.IsEmbeddedResource ||
                            asset.IsLocal ||
                            asset.IsRemoteDownload).ToList()).Distinct());

                        var renderedTag = string.Empty;
                        if (uniqueFiles.Count > 0 || bundleState.Assets.Count(a => a.IsArbitrary) > 0)
                        {
                            bundleState.DependentFiles.AddRange(uniqueFiles);

                            var minifiedContent = GetMinifiedContent(bundleState.Assets, outputFile);
                            if (!string.IsNullOrEmpty(bundleState.HashKeyName))
                            {
                                hash = hasher.GetHash(minifiedContent);
                            }

                            renderToPath = bundleState.CacheInvalidationStrategy.GetOutputWebPath(renderToPath, bundleState.HashKeyName, hash);
                            outputFile = bundleState.CacheInvalidationStrategy.GetOutputFileLocation(outputFile, hash);

                            if (!(bundleState.ShouldRenderOnlyIfOutputFileIsMissing && FileExists(outputFile)))
                            {
                                renderer.Render(minifiedContent, outputFile);
                            }

                            renderedTag = FillTemplate(bundleState, renderToPath);
                        }

                        content += String.Concat(GetFilesForRemote(remoteAssetPaths, bundleState), renderedTag);
                    }
                }
                //don't cache bundles where debugging was forced via predicate
                if (!bundleState.DebugPredicate.SafeExecute())
                {
                    bundleCache.Add(key, content, bundleState.DependentFiles, IsDebuggingEnabled());
                }
            }

            return content;
        }

        protected string GetMinifiedContent(List<Asset> assets, string outputFile)
        {
            var filteredAssets = assets.Where(asset =>
                                              asset.IsEmbeddedResource ||
                                              asset.IsLocal ||
                                              asset.IsRemoteDownload ||
                                              asset.IsArbitrary)
                                    .ToList();

            var sb = new StringBuilder();

            AggregateContent(filteredAssets, sb, outputFile);

            return sb.ToString();
        }

        protected string MinifyIfNeeded(string content, bool minify)
        {
            if (minify && !string.IsNullOrEmpty(content) && !IsDebuggingEnabled())
            {
                var minified = Minifier.Minify(content);
                return AppendFileClosure(minified);
            }
            return AppendFileClosure(content);
        }

        protected virtual string AppendFileClosure(string content)
        {
            return content;
        }

        string PreprocessForDebugging(string filename)
        {
            var preprocessors = FindPreprocessors(filename);
            if (preprocessors.NullSafeAny())
            {
                var content = PreprocessFile(filename, preprocessors);
                filename += debugExtension;
                using (var fileWriter = fileWriterFactory.GetFileWriter(filename))
                {
                    fileWriter.Write(content);
                }
            }
            return filename;
        }
    }
}