using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        internal static string MakeRelativePathTo (this Uri from, Uri to) 
        {
            var relativePath = from.MakeRelativeUri (to).OriginalString;
            if (FileSystem.Unix)
            {
                const string parentInPath = "../";
                var relativePathWithoutLeadingParents = relativePath.TrimStart (parentInPath);

                //this means URI's weren't properly combined - need to split and go up appropriate # of directories)
                if (relativePathWithoutLeadingParents.Contains (parentInPath))
                {
                    //first build the correct root path
                    var finalRoot = new StringBuilder ();

                    var parentsTrimmedFromStart = (relativePath.Length - relativePathWithoutLeadingParents.Length) / parentInPath.Length;
                    Enumerable.Range(0, parentsTrimmedFromStart)
                        .Aggregate(finalRoot, (sb, i) => sb.Append(parentInPath));

                    var piecesSeparatedByInvalidParent = relativePathWithoutLeadingParents.Split (new[] { parentInPath }, StringSplitOptions.RemoveEmptyEntries);
                    var parentsToAscend = (relativePathWithoutLeadingParents.Length - relativePathWithoutLeadingParents.Replace (parentInPath, "").Length) / 3;
                    var startPieces = piecesSeparatedByInvalidParent[0].Split (Path.DirectorySeparatorChar);

                    startPieces.Take(startPieces.Length - 1 - parentsToAscend)
                        .Aggregate(finalRoot, (sb, s) => sb.Append(s));

                    return Path.Combine (finalRoot.ToString(), piecesSeparatedByInvalidParent[1]);
                }
            }
            return relativePath;
        }

        internal static bool NullSafeAny<T>(this IEnumerable<T> values)
        {
            return values != null && values.Any();
        }
    }
}