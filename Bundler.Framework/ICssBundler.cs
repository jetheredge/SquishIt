namespace Bundler.Framework
{
    public interface ICssBundleBuilder
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder WithMedia(string media);
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
    }

    public interface ICssBundle
    {
        ICssBundleBuilder Add(string cssPath);
        string RenderNamed(string name);
    }
}