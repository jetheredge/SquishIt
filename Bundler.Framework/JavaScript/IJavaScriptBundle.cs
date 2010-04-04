namespace Bundler.Framework.JavaScript
{
    public interface IJavaScriptBundle
    {
        IJavaScriptBundleBuilder Add(string javaScriptPath);
        string RenderNamed(string name);
        void ClearCache();
    }
}