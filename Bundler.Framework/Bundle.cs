namespace Bundler.Framework
{
    public class Bundle
    {
        public static IJavaScriptBundle JavaScript()
        {
            return new JavaScriptBundle();
        }
       
        public static ICssBundle Css()
        {
            return new CssBundle();
        }
    }
}