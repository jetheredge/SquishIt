namespace Bundler.Framework
{
    public interface ICssBundler
    {
        ICssBundler AddCss(string javaScriptPath);
        string RenderCss(string renderTo);
    }
}