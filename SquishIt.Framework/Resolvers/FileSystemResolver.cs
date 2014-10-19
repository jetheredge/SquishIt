using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SquishIt.Framework.Resolvers
{
	public class FileSystemResolver : IResolver
	{
		public string Resolve(string file)
		{
			return Path.GetFullPath(file);
		}

		public bool IsDirectory(string path)
		{
			return Directory.Exists(path) || HasSearchPath(path);
		}

		bool HasSearchPath(string path)
		{
			var filename = Path.GetFileName(path);
			return filename.Contains("*") || filename.Contains("?");
		}

		string GetSearchPath(string path)
		{
			return Path.GetFileName(path);
		}

		public IEnumerable<string> ResolveFolder(string path, bool recursive, string debugFileExtension, IEnumerable<string> allowedFileExtensions, IEnumerable<string> disallowedFileExtensions)
		{
			if (IsDirectory(path))
			{
				var searchPath = "*.*";
				if(HasSearchPath(path))
				{
					searchPath = GetSearchPath(path);
					path = Path.GetDirectoryName(path);
				}

				var files = Directory.GetFiles(path, searchPath, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
					  .Where(
						  file =>
						  {
							  var f = file.ToUpperInvariant();
							  return !f.EndsWith(debugFileExtension.ToUpperInvariant()) &&
									 (allowedFileExtensions == null ||
									  allowedFileExtensions.Select(s => s.ToUpper()).Any(f.EndsWith) &&
									  IsAllowed(disallowedFileExtensions, file));
						  })
					  .ToArray();
				Array.Sort(files);
				return files;
			}
			return new[] { Path.GetFullPath(path) };
		}

		static IEnumerable<string> Extensions(string path)
		{
			return path.Split('.')
				.Skip(1)
				.Select(s => "." + s.ToUpperInvariant());
		}

		bool IsAllowed(IEnumerable<string> disallowedFileExtensions, string filePath)
		{
			if (disallowedFileExtensions == null)
				return true;

			var fileName = Path.GetFileName(filePath);

			var result = disallowedFileExtensions.Any(pattern => MatchesIgnore(pattern, fileName)) == false;
			return result;
		}

		bool MatchesIgnore(string ignorePattern, string filename)
		{
			var pattern = "^" + Regex.Escape(ignorePattern)
					  .Replace(@"\*", ".*")
					  .Replace(@"\?", ".")
			   + "$";

			return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}
	}
}