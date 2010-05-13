namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundle
    {
        IJavaScriptBundleBuilder Add(string javaScriptPath);
        string RenderNamed(string name);
        void ClearCache();
    }
}