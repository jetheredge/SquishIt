using System;
using System.IO;
using System.Linq;

namespace SquishIt.Framework.CSS
{
    internal class RelativePathAdapter
    {
        private RelativePathAdapter(string[] directoriesUp, string[] directoriesDown)
        {
            _directoriesUp = directoriesUp;
            _directoriesDown = directoriesDown;
        }

        private readonly string[] _directoriesUp;
        private readonly string[] _directoriesDown;

        public string Adapt(string relative)
        {
            var relativeWithoutLeadingAscent = relative.TrimStart("../");
            var relativeAscent = (relative.Length - relativeWithoutLeadingAscent.Length) / 3;
            var relativePathIsInSourceDirectory = relativeAscent == 0;

            var totalDirectoriesUp = _directoriesUp.Length + relativeAscent;

            var pathUp = totalDirectoriesUp == 0
                             ? string.Empty
                             : Enumerable.Range(1, totalDirectoriesUp - (relativePathIsInSourceDirectory ? 0 : _directoriesDown.Length))
                                         .Aggregate(string.Empty, (acc, i) => acc + "../");

            var pathDown = _directoriesDown
                .Take(_directoriesDown.Length - (relativePathIsInSourceDirectory ? 0 : totalDirectoriesUp))
                .Aggregate(string.Empty, (acc, s) => acc == string.Empty ? s : string.Concat(acc, "/", s));

            var pathToBase = string.Concat(pathUp, pathDown, (string.IsNullOrEmpty(pathDown) ? string.Empty : "/"));

            return (pathToBase + relativeWithoutLeadingAscent);
        }

        public static RelativePathAdapter Between(string from, string to)
        {
            var directoryFrom = Path.GetDirectoryName(from);
            var directoryTo = Path.GetDirectoryName(to);

            var commonRoot = FindRootPath(directoryFrom, directoryTo);

            var pathUp = directoryFrom.TrimStart(commonRoot);
            var pathDown = directoryTo.TrimStart(commonRoot);

            return new RelativePathAdapter(
                pathUp.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries),
                pathDown.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
        }

        static string FindRootPath(string directory1, string directory2)
        {
            var longest = directory1.Length > directory2.Length ? directory1 : directory2;
            var shortest = directory1.Length > directory2.Length ? directory2 : directory1;

            var separator = Path.DirectorySeparatorChar;
            var commonPath = String.Empty;

            var separatedPath = longest
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            foreach (var pathSegment in separatedPath)
            {
                if (commonPath.Length == 0 && shortest.StartsWith(pathSegment))
                {
                    commonPath = pathSegment;
                }
                else if (shortest.StartsWith(commonPath + separator + pathSegment))
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
    }
}