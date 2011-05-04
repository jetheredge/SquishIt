namespace SquishIt.Framework.Minifiers
{
    public interface IMinifier<T> where T : Base.BundleBase<T>
    {
        string Minify(string content);
    }
}