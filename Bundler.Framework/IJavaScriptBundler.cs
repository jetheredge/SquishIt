namespace Bundler.Framework
{
    public interface IJavaScriptBundler
    {
        IJavaScriptBundler AddJs(string javaScriptPath);
        string RenderJs(string renderTo);
    }
}