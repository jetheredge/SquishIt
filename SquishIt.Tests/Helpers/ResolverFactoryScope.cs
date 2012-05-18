using System;
using SquishIt.Framework.Resolvers;

namespace SquishIt.Tests.Helpers 
{
    public class ResolverFactoryScope : IDisposable 
    {
        public ResolverFactoryScope(string key, IResolver resolver) 
        {
            ResolverFactory.SetContent(key, resolver);
        }

        public void Dispose() 
        {
            ResolverFactory.Reset();
        }
    }
}
