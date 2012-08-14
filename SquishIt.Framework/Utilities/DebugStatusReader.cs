using System;

namespace SquishIt.Framework.Utilities
{

    public class DebugStatusReader : IDebugStatusReader
    {
        readonly IMachineConfigReader machineConfigReader;
        bool forceDebug = false;
        bool forceRelease = false;

        public DebugStatusReader()
            : this(new MachineConfigReader())
        {

        }

        internal DebugStatusReader(IMachineConfigReader machineConfigReader)
        {
            this.machineConfigReader = machineConfigReader;
        }

        public bool IsDebuggingEnabled(Func<bool> debugPredicate = null)
        {
            if(forceDebug || debugPredicate.SafeExecute())
            {
                return true;
            }

            if(forceRelease)
            {
                return false;
            }

            if(HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
            {
                return !TrustLevel.IsHighOrUnrestrictedTrust || machineConfigReader.IsNotRetailDeployment;
            }
            return false;
        }

        public void ForceDebug()
        {
            forceDebug = true;
        }

        public void ForceRelease()
        {
            forceRelease = true;
        }
    }
}