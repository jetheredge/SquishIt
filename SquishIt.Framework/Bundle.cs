using System;
using System.Collections.Generic;
using System.Linq;
using SquishIt.Framework.Base;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace SquishIt.Framework
{
    /// <summary>
    /// This is the entry point for the majority of interaction with the SquishIt API.
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// Create a javascript bundle.
        /// </summary>
        /// <returns><see cref="JavaScriptBundle">JavaScriptBundle</see></returns>
        public static JavaScriptBundle JavaScript()
        {
            return new JavaScriptBundle();
        }

        /// <summary>
        /// Create a javascript bundle with non default <see cref="IDebugStatusReader">IDebugStatusReader</see>.
        /// </summary>
        /// <param name="debugStatusReader"><see cref="IDebugStatusReader">IDebugStatusReader</see> instance to use.</param>
        /// <returns><see cref="JavaScriptBundle">JavaScriptBundle</see></returns>
        public static JavaScriptBundle JavaScript(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new JavaScriptBundle(debugStatusReader);
        }

        /// <summary>
        /// Create a CSS bundle.
        /// </summary>
        /// <returns><see cref="CSSBundle">CSSBundle</see></returns>
        public static CSSBundle Css()
        {
            return new CSSBundle();
        }

        /// <summary>
        /// Create a CSS bundle with non default <see cref="IDebugStatusReader">IDebugStatusReader</see>.
        /// </summary>
        /// <param name="debugStatusReader"><see cref="IDebugStatusReader">IDebugStatusReader</see> instance to use.</param>
        /// <returns><see cref="CSSBundle">CSSBundle</see></returns>
        public static CSSBundle Css(Utilities.IDebugStatusReader debugStatusReader)
        {
            return new CSSBundle(debugStatusReader);
        }
    }
}