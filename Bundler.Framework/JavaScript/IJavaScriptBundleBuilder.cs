namespace Bundler.Framework.JavaScript
{
    public interface IJavaScriptBundleBuilder
    {
        IJavaScriptBundleBuilder Add(string cssPath);        
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
    }
}