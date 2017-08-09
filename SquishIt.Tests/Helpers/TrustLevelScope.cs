using System;
using SquishIt.Framework;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Helpers
{
    class TrustLevelScope : IDisposable
    {
        readonly ITrustLevel previous;

        public TrustLevelScope(ITrustLevel instance)
        {
            previous = instance;
            Configuration.Instance.TrustLevel = instance;
        }

        public void Dispose()
        {
            Configuration.Instance.TrustLevel = previous;
        }
    }
}
