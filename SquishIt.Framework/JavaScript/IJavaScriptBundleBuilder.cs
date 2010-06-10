using SquishIt.Framework.JavaScript.Minifiers;

namespace SquishIt.Framework.JavaScript
{
    public interface IJavaScriptBundleBuilder
    {
        IJavaScriptBundleBuilder Add(string cssPath);
        IJavaScriptBundleBuilder AddCdn(string javaScriptPath, string cdnUri);
        IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier);
        IJavaScriptBundleBuilder RenderOnlyIfOutputFileMissing();
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
        IJavaScriptBundleBuilder ForceDebug();
        IJavaScriptBundleBuilder ForceRelease();
    }
}