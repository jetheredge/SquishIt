using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.JavaScript.Minifiers
{
	public class MsMinifier : IJavaScriptMinifier
	{
	    private readonly CodeSettings codeSettings;
	    private readonly string[] globalNames = new string[0];

	    public static string Identifier
		{
			get { return "MsJavaScriptMinifier"; }
		}

		string IJavaScriptMinifier.Identifier
		{
			get { return Identifier; }
		}

	    public MsMinifier()
	    {
	    }

	    public MsMinifier(CodeSettings codeSettings)
	    {
	        this.codeSettings = codeSettings;
	    }

	    public MsMinifier(CodeSettings codeSettings, string[] globalNames)
	    {
	        this.codeSettings = codeSettings;
	        this.globalNames = globalNames;
	    }

	    public string CompressContent(string content)
		{
			var minifer = new Minifier();
            if (codeSettings != null)
            {
                return minifer.MinifyJavaScript(content, codeSettings, globalNames);
            }
            else
            {
                return minifer.MinifyJavaScript(content, globalNames);
            }
		}
	}
}