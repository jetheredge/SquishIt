using Bundler.Framework.Css.Compressors;

namespace Bundler.Framework.Css
{
    public interface ICssBundleBuilder
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder WithMedia(string media);
        ICssBundleBuilder WithCompressor(CssCompressors cssCompressor);
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
    }
}