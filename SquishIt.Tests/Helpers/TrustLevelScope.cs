using System;
using SquishIt.Framework.Utilities;

namespace SquishIt.Tests.Helpers
{
    class TrustLevelScope : IDisposable
    {
        public TrustLevelScope(ITrustLevel instance)
        {
            TrustLevel.instance = instance;
        }

        public void Dispose()
        {
            TrustLevel.instance = null;
        }
    }
}
