namespace SquishIt.Framework
{
    public interface IPreprocessor
    {
        bool ValidFor(string filePath);
        string Process(string filePath, string content);
    }
}
