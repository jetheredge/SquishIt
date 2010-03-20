namespace Bundler.Framework
{
    public interface ICssBundle
    {
        ICssBundleBuilder Add(string cssPath);
        string RenderNamed(string name);
    }
}