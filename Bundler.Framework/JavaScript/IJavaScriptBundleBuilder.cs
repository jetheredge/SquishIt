using Bundler.Framework.JavaScript.Minifiers;

namespace Bundler.Framework.JavaScript
{
    public interface IJavaScriptBundleBuilder
    {
        IJavaScriptBundleBuilder Add(string cssPath);
        IJavaScriptBundleBuilder WithMinifier(JavaScriptMinifiers javaScriptMinifier);
        string Render(string renderTo);
        void AsNamed(string name, string renderTo);
    }
}