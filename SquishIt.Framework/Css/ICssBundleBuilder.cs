using SquishIt.Framework.Css.Compressors;

namespace SquishIt.Framework.Css
{
    public interface ICssBundleBuilder
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        ICssBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        ICssBundleBuilder WithMedia(string media);
        ICssBundleBuilder WithCompressor(CssCompressors cssCompressor);
        ICssBundleBuilder WithCompressor(ICssCompressor cssCompressor);
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
        ICssBundleBuilder RenderOnlyIfOutputFileMissing();
        ICssBundleBuilder ForceDebug();
        ICssBundleBuilder ForceRelease();
        ICssBundleBuilder ProcessImports();
        ICssBundleBuilder WithAttribute(string name, string value);
        string AsCached(string name, string cssPath);
        ICssBundleBuilder AppendHashForAssets();
        ICssBundleBuilder HashQueryStringNamed(string hashQueryStringKeyName);
    }
}