using System;
using System.IO;

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
            var combined = from.MakeRelativeUri (to);
            if (FileSystem.Unix) 
            {
                var newRelativePath = combined.OriginalString;
                var pathWithoutLeadingUps = newRelativePath.TrimStart ('.', '/');
                var leadingUps = (newRelativePath.Length - pathWithoutLeadingUps.Length) / 3;
                var directoriesUp = (pathWithoutLeadingUps.Length - pathWithoutLeadingUps.Replace ("../", "").Length) / 3;

                var finalPath = "";
                for (var i = 0; i < leadingUps; i++) 
                {
                    finalPath += "../";
                }

                if (pathWithoutLeadingUps.Contains ("../")) {
                    var pieces = pathWithoutLeadingUps.Split (new[] { "../" }, StringSplitOptions.RemoveEmptyEntries);
                    var startPieces = pieces[0].Split ('/');

                    for (var i = 0; i < (startPieces.Length - 1 - directoriesUp); i++) 
                    {
                        finalPath += startPieces[i];
                    }
                    finalPath = Path.Combine (finalPath, pieces[1]);
                    return finalPath;
                }
                return finalPath + pathWithoutLeadingUps;
            }
            return combined.OriginalString;
        }
    }
}