using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SquishIt.Framework.CSS
{
    public class CSSPathRewriter
    {
        public static string RewriteCssPaths(string outputPath, string sourcePath, string css, ICSSAssetsFileHasher cssAssetsFileHasher, bool asImport = false)
        {
            var relativePathAdapter = RelativePathAdapter.Between(outputPath, sourcePath);

            var relativePaths = FindDistinctRelativePathsIn(css);

            foreach (var relativePath in relativePaths)
            {
                var firstIndexOfHashOrQuestionMark = relativePath.IndexOfAny(new[] { '?', '#' });

                var segmentAfterHashOrQuestionMark = firstIndexOfHashOrQuestionMark >= 0
                    ? relativePath.Substring(firstIndexOfHashOrQuestionMark)
                    : string.Empty;

                var capturedRelativePath = segmentAfterHashOrQuestionMark != string.Empty
                    ? relativePath.Substring(0, firstIndexOfHashOrQuestionMark)
                    : relativePath;

                var resolvedOutput = relativePathAdapter.Adapt(capturedRelativePath);

                var newRelativePath = asImport ? "squishit://" + resolvedOutput : (resolvedOutput + segmentAfterHashOrQuestionMark);

                css = ReplaceRelativePathsIn(css, relativePath, newRelativePath);
            }

            if (!asImport)
            {
                css = css.Replace("squishit://", "");
            }

            if (cssAssetsFileHasher != null)
            {
                var localRelativePathsThatExist = FindDistinctLocalRelativePathsThatExist(css);

                foreach (var localRelativePathThatExist in localRelativePathsThatExist)
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

        static string ReplaceRelativePathsIn(string css, string oldPath, string newPath)
        {
            var regex = new Regex(@"url\(\s*[""']{0,1}" + Regex.Escape(oldPath) + @"[""']{0,1}\s*\)", RegexOptions.IgnoreCase);

            return regex.Replace(css, match =>
            {
                var path = match.Value.Replace(oldPath, newPath);
                return path;
            });
        }

        static readonly Regex pathsRegex = new Regex(@"(?<!.*behavior\s*:\s*)url\(\s*(?:[""']?)(.*?)(?:[""']?)\s*\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = pathsRegex.Matches(css);
            return matches.Cast<Match>()
                .Select(match => match.Groups[1].Captures[0].Value)
                .Where(path => !path.StartsWith("/")
                    && !path.StartsWith("http://")
                    && !path.StartsWith("https://")
                    && !path.StartsWith("data:")
                    && !path.StartsWith("squishit://")
                    && path != "\"\""
                    && path != "''"
                    && !string.IsNullOrEmpty(path)).Distinct();
        }

        static IEnumerable<string> FindDistinctLocalRelativePathsThatExist(string css)
        {
            var matches = pathsRegex.Matches(css);
            return matches.Cast<Match>()
                .Select(match => match.Groups[1].Captures[0].Value)
                .Where(path => !path.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                .Distinct();
        }
    }
}