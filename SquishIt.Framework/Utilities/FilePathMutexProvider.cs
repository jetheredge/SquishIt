using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using SquishIt.Framework.Files;

namespace SquishIt.Framework.Utilities
{
	public class FilePathMutexProvider : IFilePathMutexProvider
	{
		private const string NullPathSurrogate = "<NULL>";

		private static readonly IFilePathMutexProvider instance = new FilePathMutexProvider();
		private static readonly object createMutexLock = new object();

		private readonly Dictionary<string, Mutex> pathMutexes =
			new Dictionary<string, Mutex>(StringComparer.Ordinal);
		private readonly IHasher hasher;

		public static IFilePathMutexProvider Instance
		{
			get { return instance; }
		}

		public FilePathMutexProvider()
			: this(new Hasher(new RetryableFileOpener()))
		{
		}

		public FilePathMutexProvider(IHasher hasher)
		{
			this.hasher = hasher;
		}

		public Mutex GetMutexForPath(string path)
		{
			Mutex result;

			string normalizedPath = GetNormalizedPath(path);
			if (pathMutexes.TryGetValue(normalizedPath, out result))
			{
				return result;
			}

			lock (createMutexLock)
			{
				if (pathMutexes.TryGetValue(normalizedPath, out result))
				{
					return result;
				}

				result = CreateSharableMutexForPath(normalizedPath);
				pathMutexes[normalizedPath] = result;
			}

			return result;
		}

		private static string GetNormalizedPath(string path)
		{
			if (String.IsNullOrEmpty(path))
			{
				return NullPathSurrogate;
			}

			// Normalize the path
			var fileSystemPath = FileSystem.ResolveAppRelativePathToFileSystem(path);
			// The path is lower cased to avoid different hashes. Even on a case sensitive
			// file system this probably is okay, since it's a web application
			return Path.GetFullPath(fileSystemPath)
				.ToLowerInvariant();
		}

		private Mutex CreateSharableMutexForPath(string normalizedPath)
		{
			// The path is transformed to a hash value to avoid getting an invalid Mutex name.
			var mutexName = @"Global\SquishitPath" + hasher.GetHash(normalizedPath);
			return CreateSharableMutex(mutexName);
		}

		private static Mutex CreateSharableMutex(string name)
		{
			// Creates a mutex sharable by more than one process
			var mutexSecurity = new MutexSecurity();
			var everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
			mutexSecurity.AddAccessRule(new MutexAccessRule(everyoneSid, MutexRights.FullControl, AccessControlType.Allow));

			// The constructor will either create new mutex or open
			// an existing one, in a thread-safe manner
			bool createdNew;
			return new Mutex(false, name, out createdNew, mutexSecurity);
		}
	}
}
