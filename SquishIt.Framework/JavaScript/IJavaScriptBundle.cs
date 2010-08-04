namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundle
    {
        IJavaScriptBundleBuilder Add(string javaScriptPath);
        IJavaScriptBundleBuilder AddRemote(string localPath, string remotePath);
        string RenderNamed(string name);
        void ClearCache();
    }
}