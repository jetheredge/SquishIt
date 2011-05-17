using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using SquishIt.Framework.Minifiers;

namespace SquishIt.Framework.Resolvers
{
    public class ResolverFactory
    {
        private static Dictionary<string, IResolver> resolvers = new Dictionary<string, IResolver>
        {
            {typeof(DirectoryResolver).FullName, new DirectoryResolver()},
            {typeof(EmbeddedResourceResolver).FullName, new EmbeddedResourceResolver()},
            {typeof(FileResolver).FullName, new FileResolver()},
            {typeof(HttpResolver).FullName, new HttpResolver()},
        };

        public static T Get<T>() where T : IResolver
        {
            return (T)resolvers[typeof(T).FullName];
        }
    }
}