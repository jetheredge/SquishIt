using System.Collections.Specialized;

namespace SquishIt.Framework.Utilities
{
    public interface IQueryStringUtility
    {
        NameValueCollection Parse(string queryString);
        string Flatten(NameValueCollection queryStringCollection);
    }
}
