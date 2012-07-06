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

        public YuiMinifier(LoggingType loggingType, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval)
        {
            compressor = new JavaScriptCompressor
                             {
                                 LoggingType = loggingType,
                                 ObfuscateJavascript = obfuscateJavaScript,
                                 PreserveAllSemicolons = preserveAllSemicolons,
                                 DisableOptimizations = disableOptimizations,
                                 IgnoreEval = ignoreEval
                             };
        }

        public YuiMinifier(LoggingType loggingType, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition)
        {
            compressor = new JavaScriptCompressor
                             {
                                 LoggingType = loggingType,
                                 ObfuscateJavascript = obfuscateJavaScript,
                                 PreserveAllSemicolons = preserveAllSemicolons,
                                 DisableOptimizations = disableOptimizations,
                                 IgnoreEval = ignoreEval,
                                 LineBreakPosition = lineBreakPosition
                             };
        }

        public YuiMinifier(LoggingType loggingType, bool obfuscateJavaScript, bool preserveAllSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition, Encoding encoding, CultureInfo cultureInfo)
        {
            compressor = new JavaScriptCompressor
                             {
                                 LoggingType = loggingType,
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