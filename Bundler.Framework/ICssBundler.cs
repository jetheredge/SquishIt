namespace Bundler.Framework
{
    public interface ICssBundler
    {
        ICssBundler AddCss(string cssPath);
        string RenderCss(string renderTo);
    }
}