using Microsoft.Ajax.Utilities;

namespace SquishIt.Framework.Minifiers.JavaScript
{
	public class MsMinifier : IJavaScriptMinifier
	{
	    CodeSettings codeSettings;
	    readonly string[] globalNames = new string[0];

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

	    public string Minify(string content)
		{
			var minifer = new Minifier();
            codeSettings = codeSettings ?? new CodeSettings();
	        globalNames.ForEach(gn => codeSettings.AddKnownGlobal(gn));
            return minifer.MinifyJavaScript(content, codeSettings);
		}
	}
}