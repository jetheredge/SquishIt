using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;

namespace SquishIt.Framework.Minifiers
{
    public static class MinifierFactory
    {
        private static Dictionary<Type, Dictionary<Type, object>> Minifiers = new Dictionary<Type, Dictionary<Type, object>>
        {
            {
                typeof(CSSBundle), new Dictionary<Type, object>
                                {
                                    {typeof(CSS.MsCompressor), new CSS.MsCompressor()},
                                    {typeof(CSS.NullCompressor), new CSS.NullCompressor()},
                                    {typeof(CSS.YuiCompressor), new CSS.YuiCompressor()}
                                }
            },
            {
                typeof(JavaScriptBundle), new Dictionary<Type, object>
                                {
                                    {typeof(JavaScript.JsMinMinifier), new JavaScript.JsMinMinifier()},
                                    {typeof(JavaScript.NullMinifier), new JavaScript.NullMinifier()},
                                    {typeof(JavaScript.YuiMinifier), new JavaScript.YuiMinifier()},
                                    {typeof(JavaScript.ClosureMinifier), new JavaScript.ClosureMinifier()},
                                    {typeof(JavaScript.MsMinifier), new JavaScript.MsMinifier()}
                                }
            }        
        };

        public static Min Get<BundleType, Min>() where BundleType : Base.BundleBase<BundleType> where Min : IMinifier<BundleType>
        {
            return (Min)Minifiers[typeof(BundleType)][typeof(Min)];
        }
    }
}