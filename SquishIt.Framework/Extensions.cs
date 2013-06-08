using System;
using System.Collections.Generic;
using System.Linq;

namespace SquishIt.Framework
{
    internal static class Extensions {
        internal static string TrimStart (this string target, string toTrim) 
        {
            string result = target;
            while (result.StartsWith (toTrim)) 
            {
                result = result.Substring (toTrim.Length);
            }
            return result;
        }

        internal static bool NullSafeAny<T>(this IEnumerable<T> values)
        {
            return values != null && values.Any();
        }

        internal static bool SafeExecute(this Func<bool> function)
        {
            return function != null && function();
        }
    }
}