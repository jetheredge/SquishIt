using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.CSS
{
    public class CSSPathRewriter
    {
        public static string RewriteCssPaths(string outputPath, string sourcePath, string css, ICssAssetsFileHasher cssAssetsFileHasher)
        {
            var sourceUri = new Uri(Path.GetDirectoryName(sourcePath) + "/", UriKind.Absolute);
            var outputUri = new Uri(Path.GetDirectoryName(outputPath) + "/", UriKind.Absolute);

            var relativePaths = FindDistinctRelativePathsIn(css);

            foreach (string relativePath in  relativePaths)
            {
                var resolvedSourcePath = new Uri(sourceUri + relativePath );
                var resolvedOutput = outputUri.MakeRelativeUri(resolvedSourcePath);
                
                css = css.Replace(relativePath , resolvedOutput.OriginalString);    
            }

            if (cssAssetsFileHasher != null)
            {
                var localRelativePathsThatExist = FindDistinctLocalRelativePathsThatExist(css);

                foreach (string localRelativePathThatExist in localRelativePathsThatExist)
                {
                    var localRelativePathThatExistWithFileHash = cssAssetsFileHasher.AppendFileHash(outputPath, localRelativePathThatExist);

                    if (localRelativePathThatExist != localRelativePathThatExistWithFileHash)
                    {
                        css = css.Replace(localRelativePathThatExist, localRelativePathThatExistWithFileHash);
                    }
                }
            }
            return css;
        }
        
        private static IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = Regex.Matches(css, @"url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("/") && !path.StartsWith("http://") && !path.StartsWith("https://"))
                {
                    if (matchesHash.Add(path))
                    {
                        yield return path;
                    }
                }
            }
        }

        private static IEnumerable<string> FindDistinctLocalRelativePathsThatExist(string css)
        {
            var matches = Regex.Matches(css, @"url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (matchesHash.Add(path))
                    {
                        yield return path;
                    }
                }
            }
        }
    }
}