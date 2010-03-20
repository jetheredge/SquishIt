namespace Bundler.Framework
{
    public interface ICssBundleBuilder
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder WithMedia(string media);
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
    }
}