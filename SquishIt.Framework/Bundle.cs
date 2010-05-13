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
       
        public static ICssBundle Css()
        {
            return new CssBundle();
        }
    }
}