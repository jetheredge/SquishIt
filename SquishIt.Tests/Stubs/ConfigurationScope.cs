using System;
using SquishIt.Framework;

namespace SquishIt.Tests.Stubs
{
    public class ConfigurationScope : IDisposable
    {
        public ConfigurationScope(Configuration configuration)
        {
            Configuration.Instance = configuration;
        }
        public void Dispose()
        {
            Configuration.Instance = null;
        }
    }
}
