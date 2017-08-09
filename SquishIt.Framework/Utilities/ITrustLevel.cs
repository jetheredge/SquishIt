using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquishIt.Framework.Utilities
{
    public interface ITrustLevel
    {
        bool IsFullTrust { get; }
        bool IsHighOrUnrestrictedTrust { get; }
    }
}
