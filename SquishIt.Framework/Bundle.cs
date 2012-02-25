using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    public class Bundle
    {
        public static JavaScriptBundle JavaScript()
        {
            return JavaScript<JavaScriptBundle>();
        }

        public static JavaScriptBundle JavaScript<TBundle>()
            where TBundle : JavaScriptBundle, new()
        {
            return new TBundle();
        }        

        public static JavaScriptBundle JavaScript(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new JavaScriptBundle(debugStatusReader);
        }
       
        public static CSSBundle Css()
        {
            return new CSSBundle();
        }

        public static CSSBundle Css(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new CSSBundle(debugStatusReader);
        }
    }
}