using System.Threading;

namespace SquishIt.Framework.Utilities
{
	public interface IFilePathMutexProvider
	{
		Mutex GetMutexForPath(string path);
	}
}