using System;
using System.Collections.Generic;

namespace SquishIt.Framework.FileResolvers
{
	public class EmbeddedResourceResolver: IFileResolver
	{
		public static string Type { get { return "EmbeddedResource"; } }
		public IEnumerable<string> TryResolve(string file)
		{
			throw new NotImplementedException();
		}
	}
}