namespace SquishIt.Framework.Base
{
    public interface IRenderable
    {
        string Render(string renderTo);
        string RenderCached(string name);
        string RenderCachedAssetTag(string name);
        string RenderNamed(string name);
    }
}