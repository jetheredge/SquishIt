using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Renderers;

namespace SquishIt.Framework.Base
{
    public abstract partial class BundleBase<T> where T : BundleBase<T>
    {
        public T ForceDebug()
        {
            debugStatusReader.ForceDebug();
            bundleState.ForceDebug = true;
            return (T)this;
        }

        public T ForceRelease()
        {
            debugStatusReader.ForceRelease();
            bundleState.ForceRelease = true;
            return (T)this;
        }

        public T RenderOnlyIfOutputFileMissing()
        {
            ShouldRenderOnlyIfOutputFileIsMissing = true;
            return (T)this;
        }

        public T WithOutputBaseHref(string href)
        {
            BaseOutputHref = href;
            return (T)this;
        }

        public T WithReleaseFileRenderer(IRenderer renderer)
        {
            this.releaseFileRenderer = renderer;
            return (T)this;
        }

        public T WithAttribute(string name, string value)
        {
            AddAttributes(new Dictionary<string, string> { { name, value } });
            return (T)this;
        }

        public T WithAttributes(Dictionary<string, string> attributes, bool merge = true)
        {
            AddAttributes(attributes, merge: merge);
            return (T)this;
        }

        public T WithMinifier<TMin>() where TMin : IMinifier<T>
        {
            return WithMinifier(MinifierFactory.Get<T, TMin>());
        }

        public T WithMinifier<TMin>(TMin instance) where TMin : IMinifier<T>
        {
            Minifier = instance;
            return (T)this;
        }

        private string FillTemplate(BundleState state, string path)
        {
            return string.Format(Template, GetAdditionalAttributes(state), path);
        }

        public T HashKeyNamed(string hashQueryStringKeyName)
        {
            HashKeyName = hashQueryStringKeyName;
            return (T)this;
        }

        public T WithoutRevisionHash()
        {
            return HashKeyNamed(string.Empty);
        }

        public T WithPreprocessor(IPreprocessor instance)
        {
            if(!instancePreprocessors.Any(ipp => ipp.GetType() == instance.GetType()))
            {
                foreach(var extension in instance.Extensions)
                {
                    instanceAllowedExtensions.Add(extension);
                }
                instancePreprocessors.Add(instance);
            }
            return (T)this;
        }

        public T Add(string fileOrFolderPath)
        {
            AddAsset(new Asset(fileOrFolderPath));
            return (T)this;
        }

        public T AddDirectory(string folderPath, bool recursive = true)
        {
            AddAsset(new Asset(folderPath, isRecursive: recursive));
            return (T)this;
        }

        public T AddString(string content)
        {
            return AddString(content, defaultExtension);
        }

        public T AddString(string content, string extension)
        {
            if(!bundleState.Arbitrary.Any(ac => ac.Content == content))
                bundleState.Arbitrary.Add(new ArbitraryContent { Content = content, Extension = extension });
            return (T)this;
        }

        public T AddString(string format, object[] values)
        {
            return AddString(format, defaultExtension, values);
        }

        public T AddString(string format, string extension, object[] values)
        {
            var content = string.Format(format, values);
            return AddString(content, extension);
        }

        public T AddRemote(string localPath, string remotePath)
        {
            return AddRemote(localPath, remotePath, false);
        }

        public T AddRemote(string localPath, string remotePath, bool downloadRemote)
        {
            var asset = new Asset(localPath, remotePath);
            asset.DownloadRemote = downloadRemote;
            AddAsset(asset);
            return (T)this;
        }

        public T AddDynamic(string siteRelativePath)
        {
            var absolutePath = BuildAbsolutePath(siteRelativePath);
            return AddRemote(siteRelativePath, absolutePath, true);
        }

        public T AddEmbeddedResource(string localPath, string embeddedResourcePath)
        {
            AddAsset(new Asset(localPath, embeddedResourcePath, 0, true));
            return (T)this;
        }
    }
}
