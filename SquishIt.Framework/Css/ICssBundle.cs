namespace SquishIt.Framework.Css
{
    public interface ICssBundle
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        string RenderNamed(string name);
        void ClearCache();
    }
}