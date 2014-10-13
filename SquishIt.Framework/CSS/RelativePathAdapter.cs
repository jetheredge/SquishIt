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
            var sourceIsDeeperThanDestination = _directoriesUp.Length < _directoriesDown.Length;

            var totalDirectoriesUp = _directoriesUp.Length + relativeAscent;

            var howFarToAscend = sourceIsDeeperThanDestination ? _directoriesUp.Length : Math.Max(totalDirectoriesUp - (relativePathIsInSourceDirectory ? 0 : _directoriesDown.Length), 0);
            var howFarToDescend = _directoriesDown.Length - (relativePathIsInSourceDirectory ? 0 : sourceIsDeeperThanDestination ? relativeAscent : totalDirectoriesUp);

            var pathUp = totalDirectoriesUp == 0
                                ? string.Empty
                                : Enumerable.Range(1, howFarToAscend)
                                            .Aggregate(string.Empty, (acc, i) => acc + "../");

            var pathDown = _directoriesDown
                .Take(howFarToDescend)
                .Aggregate(string.Empty, (acc, s) => acc == string.Empty ? s : string.Concat(acc, "/", s));

            var pathToBase = string.Concat(pathUp, pathDown, (string.IsNullOrEmpty(pathDown) ? string.Empty : "/"));

            return (pathToBase + relativeWithoutLeadingAscent);
        }

        public static RelativePathAdapter Between(string from, string to)
        {
            var commonRoot = FindRootPath(from, to);

            if (string.IsNullOrEmpty(commonRoot))
            {
                throw new InvalidOperationException(string.Format("Can't calculate relative distance between '{0}' and '{1}' because they do not have a shared base.", from, to));
            }

            var pathUp = from.TrimStart(commonRoot);
            var pathDown = to.TrimStart(commonRoot);

            return new RelativePathAdapter(
                pathUp.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries),
                pathDown.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
        }

        static string FindRootPath(string directory1, string directory2)
        {
            var longest = directory1.Length > directory2.Length ? directory1 : directory2;
            var shortest = directory1.Length > directory2.Length ? directory2 : directory1;

            const char separator = '/';
            var commonPath = new string(separator, 1);//when working with web paths within an application, should always have shared root of '/'

            var separatedPath = longest
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            foreach (var pathSegment in separatedPath)
            {
                if (commonPath.Length == 1 && shortest.StartsWith(separator + pathSegment))
                {
                    commonPath += pathSegment;
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
