namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundle
    {
        IJavaScriptBundleBuilder Add(string javaScriptPath);
        IJavaScriptBundleBuilder AddCdn(string javaScriptPath, string cdnUri);
        string RenderNamed(string name);
        void ClearCache();
    }
}