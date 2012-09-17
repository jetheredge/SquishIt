namespace SquishIt.Framework
{
    public interface IPreprocessor
    {
        bool ValidFor(string extension);
        IProcessResult Process(string filePath, string content);
        string[] Extensions { get; }
    }
}
