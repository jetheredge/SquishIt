namespace SquishIt.Framework.Css.Compressors
{
    public class NullCompressor: ICssCompressor
    {
        public static string Identifier
        {
            get { return "NullCompressor"; }
        }

        public string CompressContent(string content)
        {
            return content;
        }

        public string CompressContent(string content, bool removeComments)
        {
            return content;
        }

        string ICssCompressor.Identifier
        {
            get { return Identifier; }
        }        
    }
}