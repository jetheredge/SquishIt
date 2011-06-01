namespace SquishIt.Framework.Files
{
    public class InputFile
    {
        public string FilePath { get; private set; }
        public Resolvers.IResolver Resolver { get; private set; }
	
        public InputFile(string filePath, Resolvers.IResolver resolver)
        {
            FilePath = filePath;
            Resolver = resolver;
        }
    }
}