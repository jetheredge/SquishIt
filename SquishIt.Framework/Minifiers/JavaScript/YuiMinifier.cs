using Yahoo.Yui.Compressor;
using System.Text;
using System.Globalization;

namespace SquishIt.Framework.Minifiers.JavaScript
{
    public class YuiMinifier: IJavaScriptMinifier
    {
        readonly bool verboseLogging = true;
        readonly bool obfuscateJavaScript = true;
        readonly bool preserveAllSemicolons = false;
        readonly bool disableOptimizations = false;
        readonly int lineBreakPosition = -1;
        readonly Encoding encoding = Encoding.UTF8;
        readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        readonly bool ignoreEval = false;

        public YuiMinifier()
        {
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval)
        {
            this.verboseLogging = verboseLogging;
            this.obfuscateJavaScript = obfuscateJavaScript;
            this.preserveAllSemicolons = preserveAllSemicolons;
            this.disableOptimizations = disableOptimizations;
            this.ignoreEval = ignoreEval;
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition)
        {
            this.verboseLogging = verboseLogging;
            this.obfuscateJavaScript = obfuscateJavaScript;
            this.preserveAllSemicolons = preserveAllSemicolons;
            this.disableOptimizations = disableOptimizations;
            this.ignoreEval = ignoreEval;
            this.lineBreakPosition = lineBreakPosition;
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition, Encoding encoding, CultureInfo cultureInfo)
        {
            this.verboseLogging = verboseLogging;
            this.obfuscateJavaScript = obfuscateJavaScript;
            this.preserveAllSemicolons = preserveAllSemicolons;
            this.disableOptimizations = disableOptimizations;
            this.lineBreakPosition = lineBreakPosition;
            this.encoding = encoding;
            this.cultureInfo = cultureInfo;
            this.ignoreEval = ignoreEval;
        }

        public string Minify(string content)
        {
            return JavaScriptCompressor.Compress(content, verboseLogging, obfuscateJavaScript, preserveAllSemicolons, disableOptimizations, lineBreakPosition, encoding, cultureInfo, ignoreEval);
        }
    }
}