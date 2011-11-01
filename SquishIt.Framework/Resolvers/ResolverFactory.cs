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
            {typeof(EmbeddedResourceResolver).FullName, new EmbeddedResourceResolver()},
            {typeof(FileSystemResolver).FullName, new FileSystemResolver()},
            {typeof(HttpResolver).FullName, new HttpResolver()},
        };

        public static T Get<T>() where T : IResolver
        {
            return (T)resolvers[typeof(T).FullName];
        }
        
        internal void SetContent<T>(T resolver) where T : IResolver 
        {
            var key = typeof(T).FullName; 
            if(resolvers.ContainsKey(key))
            {
                resolvers[key] = resolver;   
            }
            else 
            {
                resolvers.Add(key, resolver);   
            }
        }
    }
}