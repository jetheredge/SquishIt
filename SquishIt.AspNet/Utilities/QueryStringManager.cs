using System.Collections.Specialized;
using System.Web;
using SquishIt.Framework.Utilities;

namespace SquishIt.AspNet.Utilities
{
    public class QueryStringManager : IQueryStringManager
    {
        public NameValueCollection ParseQueryString(string queryString)
        {
            return HttpUtility.ParseQueryString(queryString);
        }

        //workaround for mono bug - queryString.ToString() above was returning "System.Collections.Specialized.NameValueCollection"
        public string FlattenQueryString(System.Collections.Specialized.NameValueCollection queryString)
        {
            var output = new System.Text.StringBuilder();
            for (int i = 0; i < queryString.Count; i++)
            {
                if (i > 0) output.Append("&");
                output.Append(queryString.AllKeys[i]);
                output.Append("=");
                output.Append(queryString[i]);
            }
            return output.ToString();
        }
    }
}
