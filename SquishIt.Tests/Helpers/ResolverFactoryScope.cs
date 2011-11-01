using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquishIt.Framework.Resolvers;

namespace SquishIt.Tests.Helpers 
{
    public class ResolverFactoryScope : IDisposable 
    {
        public ResolverFactoryScope(string key, IResolver resolver) 
        {
            SquishIt.Framework.Resolvers.ResolverFactory.SetContent(key, resolver);
        }

        public void Dispose() 
        {
            SquishIt.Framework.Resolvers.ResolverFactory.Reset();
        }
    }
}
