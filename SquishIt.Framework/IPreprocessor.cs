namespace SquishIt.Framework
{
    public interface IPreprocessor
    {
        bool ValidFor(string extension);
        string Process(string filePath, string content);
        string[] Extensions { get; }
    }
}
