using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    public class CSSPathRewriter
    {
        public static string RewriteCssPaths (string outputPath, string sourcePath, string css, ICssAssetsFileHasher cssAssetsFileHasher, bool asImport = false) {
            //see http://stackoverflow.com/questions/3692818/uri-makerelativeuri-behavior-on-mono
            if (FileSystem.Unix) 
            {
                outputPath += "/";
            }

            var sourceDirectory = Path.GetDirectoryName (sourcePath) + "/";
            var outputUri = new Uri (Path.GetDirectoryName (outputPath) + "/", UriKind.Absolute);

            var relativePaths = FindDistinctRelativePathsIn (css);

            foreach (string relativePath in relativePaths) 
            {
                var resolvedSourcePathString = Path.Combine (sourceDirectory, relativePath);

                var resolvedSourcePath = new Uri (resolvedSourcePathString);

                var resolvedOutput = ApplyUnixFix (outputUri.MakeRelativeUri (resolvedSourcePath).OriginalString);

                var newRelativePath = asImport ? "squishit://" + resolvedOutput : resolvedOutput;

                css = ReplaceRelativePathsIn (css, relativePath, newRelativePath);
            }

            if (!asImport) 
            {
                css = css.Replace ("squishit://", "");
            }

            if (cssAssetsFileHasher != null) 
            {
                var localRelativePathsThatExist = FindDistinctLocalRelativePathsThatExist (css);

                foreach (string localRelativePathThatExist in localRelativePathsThatExist) 
                {
                    var localRelativePathThatExistWithFileHash = cssAssetsFileHasher.AppendFileHash (outputPath, localRelativePathThatExist);

                    if (localRelativePathThatExist != localRelativePathThatExistWithFileHash) 
                    {
                        css = css.Replace (localRelativePathThatExist, localRelativePathThatExistWithFileHash);
                    }
                }
            }
            return css;
        }

        static string ApplyUnixFix (string newRelativePath) 
        {
            if (FileSystem.Unix) 
            {
                var pathWithoutLeadingUps = newRelativePath.TrimStart ('.', '/');
                var leadingUps = (newRelativePath.Length - pathWithoutLeadingUps.Length) / 3;
                var directoriesUp = (pathWithoutLeadingUps.Length - pathWithoutLeadingUps.Replace ("../", "").Length) / 3;

                var finalPath = "";
                for (var i = 0; i < leadingUps; i++) 
                {
                    finalPath += "../";
                }

                if (pathWithoutLeadingUps.Contains ("../")) 
                {
                    var pieces = pathWithoutLeadingUps.Split (new[] { "../" }, StringSplitOptions.RemoveEmptyEntries);
                    var startPieces = pieces[0].Split ('/');

                    for (var i = 0; i < (startPieces.Length - 1 - directoriesUp); i++) 
                    {
                        finalPath += startPieces[i];
                    }
                    finalPath = Path.Combine (finalPath, pieces[1]);
                }
                else 
                {
                    finalPath += pathWithoutLeadingUps;
                }
                newRelativePath = finalPath;
            }
            return newRelativePath;
        }

        private static string ReplaceRelativePathsIn(string css, string oldPath, string newPath)
        {
            var regex = new Regex(@"url\([""']{0,1}" + Regex.Escape(oldPath) + @"[""']{0,1}\)", RegexOptions.IgnoreCase);

            return regex.Replace(css, match =>
            {
                var path = match.Value.Replace(oldPath, newPath);
                return path;
            });
        }

        private static IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = Regex.Matches(css, @"url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("/") && !path.StartsWith("http://") && !path.StartsWith("https://") && !path.StartsWith("data:") && !path.StartsWith("squishit://"))
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