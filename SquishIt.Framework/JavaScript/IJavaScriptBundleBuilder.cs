using SquishIt.Framework.JavaScript.Minifiers;

namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundleBuilder
    {
        IJavaScriptBundleBuilder Add(string path);
        IJavaScriptBundleBuilder AddRemote(string localPath, string remotePath);
        IJavaScriptBundleBuilder AddEmbeddedResource(string localPath, string embeddedResourcePath);
        IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier);
        IJavaScriptBundleBuilder WithMinifier(IJavaScriptMinifier javaScriptMinifier);
        IJavaScriptBundleBuilder RenderOnlyIfOutputFileMissing();
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
        IJavaScriptBundleBuilder ForceDebug();
        IJavaScriptBundleBuilder ForceRelease();
        IJavaScriptBundleBuilder WithAttribute(string name, string value);
        string AsCached(string name, string cssPath);
    }
}