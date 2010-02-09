namespace JavascriptBundler.Files
{
    public class InputFile
    {
        public string FilePath { get; private set; }
        public string FileType { get; private set; }

        public InputFile(string filePath, string fileType)
        {
            FilePath = filePath;
            FileType = fileType;
        }
    }
}