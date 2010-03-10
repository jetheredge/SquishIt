namespace Bundler.Framework
{
    public class Bundle
    {
        public static IJavaScriptBundler JavaScript()
        {
            return new JavaScriptBundle();
        }
       
        public static ICssBundler Css()
        {
            return new CssBundle();
        }
    }
}