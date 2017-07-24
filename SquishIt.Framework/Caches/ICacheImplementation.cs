namespace SquishIt.Framework.Caches
{
    public interface ICacheImplementation
    {
        object Add(string key, object value, string[] fileDependencies, bool debuggingEnabled);
        bool ContainsKey(string key);
        object Get(string key);
        void Remove(string key);
    }
}