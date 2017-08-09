using System.Collections.Specialized;

namespace SquishIt.Framework.Utilities
{
    public interface IQueryStringManager
    {
        NameValueCollection ParseQueryString(string queryString);
        string FlattenQueryString(NameValueCollection queryStringCollection);
    }
}
