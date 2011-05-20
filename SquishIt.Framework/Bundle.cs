using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    public class Bundle
    {
        public static IJavaScriptBundle JavaScript()
        {
            return new JavaScriptBundle();
        }

        public static IJavaScriptBundle JavaScript(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new JavaScriptBundle(debugStatusReader);
        }
       
        public static ICssBundle Css()
        {
            return new CssBundle();
        }

        public static ICssBundle Css(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new CssBundle(debugStatusReader);
        }
    }
}