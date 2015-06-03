using System;
using System.Collections.Generic;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework.Minifiers
{
    public static class MinifierFactory
    {
        static readonly Dictionary<Type, Dictionary<Type, object>> Minifiers = new Dictionary<Type, Dictionary<Type, object>>
        {
            {
                typeof(CSSBundle), new Dictionary<Type, object>
                                {
                                    {typeof(CSS.MsMinifier), new CSS.MsMinifier()},
                                    {typeof(CSS.NullMinifier), new CSS.NullMinifier()},
                                    {typeof(CSS.YuiMinifier), new CSS.YuiMinifier()}
                                }
            },
            {
                typeof(JavaScriptBundle), new Dictionary<Type, object>
                                {
                                    {typeof(JavaScript.JsMinMinifier), new JavaScript.JsMinMinifier()},
                                    {typeof(JavaScript.NullMinifier), new JavaScript.NullMinifier()},
                                    {typeof(JavaScript.YuiMinifier), new JavaScript.YuiMinifier()},
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