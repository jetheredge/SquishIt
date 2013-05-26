using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SquishIt.Framework.CSS
{
    public class CSSPathRewriter
    {
        public static string RewriteCssPaths(string outputPath, string sourcePath, string css, ICSSAssetsFileHasher cssAssetsFileHasher, bool asImport = false)
        {
            var difference = CalculateDifference(outputPath, sourcePath);

            var relativePaths = FindDistinctRelativePathsIn(css);

            foreach (string relativePath in relativePaths)
            {
                var firstIndexOfHashOrQuestionMark = relativePath.IndexOfAny(new[] { '?', '#' });

                var segmentAfterHashOrQuestionMark = firstIndexOfHashOrQuestionMark >= 0
                    ? relativePath.Substring(firstIndexOfHashOrQuestionMark)
                    : string.Empty;

                var capturedRelativePath = segmentAfterHashOrQuestionMark != string.Empty
                    ? relativePath.Substring(0, firstIndexOfHashOrQuestionMark)
                    : relativePath;

                var resolvedOutput = difference.ApplyTo(capturedRelativePath);

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

        static DirectoryDifference CalculateDifference(string from, string to)
        {
            var directoryFrom = Path.GetDirectoryName(from);
            var directoryTo = Path.GetDirectoryName(to);

            var commonRoot = FindRootPath(new List<string> { directoryFrom, directoryTo });

            var directoriesUp =
                commonRoot == directoryFrom
                    ? 0
                    : directoryFrom.TrimStart(commonRoot).Count(c => c == Path.DirectorySeparatorChar);


            var pathUp = directoryFrom.TrimStart(commonRoot);
            var pathDown = directoryTo.TrimStart(commonRoot);

            return new DirectoryDifference(
                pathDown.Split(new[] { Path.DirectorySeparatorChar },StringSplitOptions.RemoveEmptyEntries), 
                pathUp.Split(new[] { Path.DirectorySeparatorChar },StringSplitOptions.RemoveEmptyEntries));
        }

        static string FindRootPath(List<string> paths)
        {
            var separator = Path.DirectorySeparatorChar;
            string commonPath = String.Empty;
            List<string> separatedPath = paths
                .First(str => str.Length == paths.Max(st2 => st2.Length))
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            foreach (string pathSegment in separatedPath.AsEnumerable())
            {
                if (commonPath.Length == 0 && paths.All(str => str.StartsWith(pathSegment)))
                {
                    commonPath = pathSegment;
                }
                else if (paths.All(str => str.StartsWith(commonPath + separator + pathSegment)))
                {
                    commonPath += separator + pathSegment;
                }
                else
                {
                    break;
                }
            }

            return commonPath;
        }

        internal class DirectoryDifference
        {
            public DirectoryDifference(string[] localDirectoriesDown, string[] localDirectoriesUp)
            {
                _directoriesDown = localDirectoriesDown;
                _directoriesUp = localDirectoriesUp;
            }

            private readonly string[] _directoriesUp;
            private readonly string[] _directoriesDown;

            public string ApplyTo(string relative)
            {
                //TODO: make this somewhat readable/sensible if it works
                var localRelative = relative.TrimStart("../");

                var relativeDirectoriesUp = (relative.Length - localRelative.Length) / 3;

                var isInSource = relativeDirectoriesUp == 0;

                var totalDirectoriesUp = _directoriesUp.Length + relativeDirectoriesUp;

                var localLead = _directoriesDown
                    .Take(_directoriesDown.Length - (isInSource ? 0 : totalDirectoriesUp)).ToArray();

                var s = totalDirectoriesUp > 0 ?
                    Enumerable.Range(1, totalDirectoriesUp - (isInSource ? 0 : _directoriesDown.Length)).Aggregate(string.Empty, (acc, i) => acc + "../") : string.Empty;

                var d = string.Join("/", localLead);
                var o = s + d;
                var x = o.Length == 0 ? o : o + "/";
                return (x + localRelative).Replace("//", "/");
            }
        }
    }
}