namespace Bundler.Framework
{
    public interface IJavaScriptBundle
    {
        IJavaScriptBundleBuilder Add(string javaScriptPath);
        string RenderNamed(string name);
    }
}