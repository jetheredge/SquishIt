using Yahoo.Yui.Compressor;
using System.Text;
using System.Globalization;

namespace SquishIt.Framework.Minifiers.JavaScript
{
    public class YuiMinifier : IJavaScriptMinifier
    {
        readonly JavaScriptCompressor compressor;

        public YuiMinifier()
        {
            compressor = new JavaScriptCompressor();
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval)
        {
            compressor = new JavaScriptCompressor();
            compressor.LoggingType = verboseLogging ? LoggingType.Debug : LoggingType.None;
            compressor.ObfuscateJavascript = obfuscateJavaScript;
            compressor.PreserveAllSemicolons = preserveAllSemicolons;
            compressor.DisableOptimizations = disableOptimizations;
            compressor.IgnoreEval = ignoreEval;
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition)
        {
            compressor = new JavaScriptCompressor();
            compressor.LoggingType = verboseLogging ? LoggingType.Debug : LoggingType.None;
            compressor.ObfuscateJavascript = obfuscateJavaScript;
            compressor.PreserveAllSemicolons = preserveAllSemicolons;
            compressor.DisableOptimizations = disableOptimizations;
            compressor.IgnoreEval = ignoreEval;
            compressor.LineBreakPosition = lineBreakPosition;
        }

        public YuiMinifier(bool verboseLogging, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition, Encoding encoding, CultureInfo cultureInfo)
        {
            compressor = new JavaScriptCompressor
                             {
                                 LoggingType = verboseLogging ? LoggingType.Debug : LoggingType.None,
                                 ObfuscateJavascript = obfuscateJavaScript,
                                 PreserveAllSemicolons = preserveAllSemicolons,
                                 DisableOptimizations = disableOptimizations,
                                 LineBreakPosition = lineBreakPosition,
                                 Encoding = encoding,
                                 ThreadCulture = cultureInfo,
                                 IgnoreEval = ignoreEval
                             };
        }

        public string Minify(string content)
        {
            return compressor.Compress(content);
        }
    }
}