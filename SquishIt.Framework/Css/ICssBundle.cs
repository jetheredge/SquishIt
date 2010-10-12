namespace SquishIt.Framework.Css
{
    public interface ICssBundle
    {
        ICssBundleBuilder Add(string cssPath);
        ICssBundleBuilder AddRemote(string localPath, string remotePath);
        ICssBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        string RenderNamed(string name);
        void ClearCache();
        string RenderCached(string name);
    }
}