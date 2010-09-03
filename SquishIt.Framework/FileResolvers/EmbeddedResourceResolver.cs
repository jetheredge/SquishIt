using System;
using System.Collections.Generic;

namespace SquishIt.Framework.FileResolvers
{
	public class EmbeddedResourceResolver: IFileResolver
	{
		public static string Type { get; set; }
		public IEnumerable<string> TryResolve(string file)
		{
			throw new NotImplementedException();
		}
	}
}