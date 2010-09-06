using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.JavaScript.Minifiers
{
	public class MsMinifier : IJavaScriptCompressor
	{
		public static string Identifier
		{
			get { return "MsJavaScriptMinifier"; }
		}

		string IJavaScriptCompressor.Identifier
		{
			get { return Identifier; }
		}

		public string CompressContent(string content)
		{
			var minifer = new Minifier();
			var settings = new CodeSettings();
			return minifer.MinifyJavaScript(content);
		}
	}
}