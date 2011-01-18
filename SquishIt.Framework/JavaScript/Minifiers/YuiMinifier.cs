using System;
using Yahoo.Yui.Compressor;
using System.Text;
using System.Globalization;

namespace SquishIt.Framework.JavaScript.Minifiers
{
    public class YuiMinifier: IJavaScriptMinifier
    {
        private readonly bool verboseLogging = true;
        private readonly bool obfuscateJavaScript = true;
        private readonly bool preserveAllSemicolons = false;
        private readonly bool disableOptimizations = false;
        private readonly int lineBreakPosition = -1;
        private readonly Encoding encoding = Encoding.UTF8;
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        private readonly bool ignoreEval = false;

        string IJavaScriptMinifier.Identifier
        {
            get { return Identifier; }
        }
        
        public static string Identifier
        {
            get { return "YuiJavaScriptCompressor"; }
        }

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

        public string CompressContent(string content)
        {
            return JavaScriptCompressor.Compress(content, verboseLogging, obfuscateJavaScript, preserveAllSemicolons, disableOptimizations, lineBreakPosition, encoding, cultureInfo, ignoreEval);
        }
    }
}