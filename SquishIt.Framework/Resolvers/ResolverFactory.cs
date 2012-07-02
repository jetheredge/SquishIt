using System;
using System.Collections.Generic;

namespace SquishIt.Framework.Resolvers
{
    public class ResolverFactory
    {
        static Dictionary<string, IResolver> resolvers = new Dictionary<string, IResolver>
        {
            {typeof(EmbeddedResourceResolver).FullName, new EmbeddedResourceResolver()},
            {typeof(FileSystemResolver).FullName, new FileSystemResolver()},
            {typeof(HttpResolver).FullName, new HttpResolver()},
        };
        
        public static IResolver Get<T>() where T : IResolver
        {
            return resolvers[typeof(T).FullName];
        }
        
        internal static void SetContent(string key, IResolver resolver) 
        {
            if(resolvers.ContainsKey(key))
            {
                resolvers[key] = resolver;   
            }
            else 
            {
                throw new InvalidOperationException("Invalid resolver type injected");  
            }
        }
        
        internal static void Reset() 
        {
            resolvers = new Dictionary<string, IResolver>
            {
                {typeof(EmbeddedResourceResolver).FullName, new EmbeddedResourceResolver()},
                {typeof(FileSystemResolver).FullName, new FileSystemResolver()},
                {typeof(HttpResolver).FullName, new HttpResolver()},
            };
        }
    }
}