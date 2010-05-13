namespace SquishIt.Framework.Css
{
    public interface ICssBundle
    {
        ICssBundleBuilder Add(string cssPath);
        string RenderNamed(string name);
        void ClearCache();
    }
}