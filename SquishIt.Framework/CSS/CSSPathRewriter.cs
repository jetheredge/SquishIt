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
            var relativePaths = FindDistinctRelativePathsIn(css);

            if (relativePaths.Any())
            {
                var relativePathAdapter = RelativePathAdapter.Between(outputPath, sourcePath);

                foreach (var relativePath in relativePaths)
                {
                    var firstIndexOfHashOrQuestionMark = relativePath.IndexOfAny(new[] {'?', '#'});

                    var segmentAfterHashOrQuestionMark = firstIndexOfHashOrQuestionMark >= 0
                                                             ? relativePath.Substring(firstIndexOfHashOrQuestionMark)
                                                             : string.Empty;

                    var capturedRelativePath = segmentAfterHashOrQuestionMark != string.Empty
                                                   ? relativePath.Substring(0, firstIndexOfHashOrQuestionMark)
                                                   : relativePath;

                    var resolvedOutput = relativePathAdapter.Adapt(capturedRelativePath);

                    var newRelativePath = (asImport ? "squishit://" : "") +  (resolvedOutput + segmentAfterHashOrQuestionMark);

                    css = ReplaceRelativePathsIn(css, relativePath, newRelativePath);
                }
            }

            //moved out of if block above so that root-relative paths can be hashed as well
            if(cssAssetsFileHasher != null)
            {
                var hashableAssetPaths = FindHashableAssetPaths(css);

                foreach(var hashableAssetPath in hashableAssetPaths)
                {
                    var localRelativePathThatExistWithFileHash = cssAssetsFileHasher.AppendFileHash(outputPath, hashableAssetPath);

                    if(hashableAssetPath != localRelativePathThatExistWithFileHash)
                    {
                        css = css.Replace(hashableAssetPath, localRelativePathThatExistWithFileHash);
                    }
                }
            }

            if(!asImport)
            {
                css = css.Replace("squishit://", "");
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

        static readonly Regex relativePathsRegex = new Regex(@"(?<!.*behavior\s*:\s*)url\(\s*(?:[""']?)(.*?)(?:[""']?)\s*\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static IList<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = relativePathsRegex.Matches(css);
            return matches.Cast<Match>()
                          .Select(match => match.Groups[1].Captures[0].Value)
                          .Where(path => !path.StartsWith("/")
                                         && !path.StartsWith("http://")
                                         && !path.StartsWith("https://")
                                         && !path.StartsWith("data:")
                                         && !path.StartsWith("squishit://")
                                         && path != "\"\""
                                         && path != "''"
                                         && !string.IsNullOrEmpty(path))
                          .Distinct()
                          .ToList();
        }

        static IEnumerable<string> FindHashableAssetPaths(string css)
        {
            //TODO: look to hash root relative paths as well eg /Images/foo.png (https://github.com/jetheredge/SquishIt/issues/292)
            var matches = relativePathsRegex.Matches(css);
            return matches.Cast<Match>()
                          .Select(match => match.Groups[1].Captures[0].Value)
                          .Where(path => !path.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                                      && !path.StartsWith("data:"))
                          .Distinct();
        }
    }
}