using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SquishIt.Framework.Css
{
    public class CssPathRewriter
    {
        public static string RewriteCssPaths(string outputPath, string sourcePath, string css)
        {
            var sourceUri = new Uri(Path.GetDirectoryName(sourcePath) + "/", UriKind.Absolute);
            var outputUri = new Uri(Path.GetDirectoryName(outputPath) + "/", UriKind.Absolute);

            var matches = Regex.Matches(css, @"url\(""{0,1}(.+?)""{0,1}\)", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                var capture = match.Groups[1].Captures[0];
                if (!capture.Value.StartsWith("/"))
                {
                    var resolvedSourcePath = new Uri(sourceUri + capture.Value);
                    var resolvedOutput = outputUri.MakeRelativeUri(resolvedSourcePath);
                    //capture.Value
                    css = css.Replace(capture.Value, resolvedOutput.OriginalString);    
                }
            }
            return css;
        }
    }
}