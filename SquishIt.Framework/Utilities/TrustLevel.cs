using System.Web;

namespace SquishIt.Framework.Utilities
{
    public interface ITrustLevel
    {
        AspNetHostingPermissionLevel CurrentTrustLevel { get; }
    }

    class TrustLevel : ITrustLevel
    {
        internal static ITrustLevel instance;
        static ITrustLevel Instance
        {
            get { return instance ?? (instance = new TrustLevel()); }
        }

        public static bool IsHighOrUnrestrictedTrust
        {
            get { return Instance.CurrentTrustLevel == AspNetHostingPermissionLevel.High || Instance.CurrentTrustLevel == AspNetHostingPermissionLevel.Unrestricted; }
        }

        AspNetHostingPermissionLevel? trustLevel;

        public AspNetHostingPermissionLevel CurrentTrustLevel
        {
            get
            {
                if(trustLevel == null)
                {
                    var lastTrustedLevel = AspNetHostingPermissionLevel.None;

                    foreach(var level in new[] {
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
                        catch(System.Security.SecurityException)
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
