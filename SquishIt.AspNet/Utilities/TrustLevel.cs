using System.Web;
using SquishIt.Framework.Utilities;

namespace SquishIt.AspNet.Utilities
{

    class TrustLevel : ITrustLevel
    {
        public bool IsFullTrust
        {
            get
            {
                return this.CurrentTrustLevel == AspNetHostingPermissionLevel.Unrestricted;
            }
        }

        public bool IsHighOrUnrestrictedTrust
        {
            get
            {
                return this.CurrentTrustLevel == AspNetHostingPermissionLevel.High ||
                    this.CurrentTrustLevel == AspNetHostingPermissionLevel.Unrestricted;
            }
        }

        AspNetHostingPermissionLevel? trustLevel;

        AspNetHostingPermissionLevel CurrentTrustLevel
        {
            get
            {
                if (trustLevel == null)
                {
                    var lastTrustedLevel = AspNetHostingPermissionLevel.None;

                    foreach (var level in new[] {
                                          AspNetHostingPermissionLevel.Minimal,
                                          AspNetHostingPermissionLevel.Low,
                                          AspNetHostingPermissionLevel.Medium,
                                          AspNetHostingPermissionLevel.High,
                                          AspNetHostingPermissionLevel.Unrestricted
                                      })
                    {
                        try
                        {
                            new AspNetHostingPermission(level).Demand();
                            lastTrustedLevel = level;
                        }
                        catch (System.Security.SecurityException)
                        {
                            break;
                        }
                    }

                    trustLevel = lastTrustedLevel;
                }
                return trustLevel.Value;
            }
        }
    }
}
