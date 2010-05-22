using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace SquishIt.Framework.Utilities
{
    public class DebugStatusReader: IDebugStatusReader
    {
        private bool forceDebug = false;
        private bool forceRelease = false;

        public bool IsDebuggingEnabled()
        {
            if (forceDebug)
            {
                return true;
            }

            if (forceRelease)
            {
                return false;
            }
            
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
            {                
                //check retail setting in machine.config
                //Thanks Dave Ward! http://www.encosia.com
                Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
                var group = machineConfig.GetSectionGroup("system.web");
                if (group != null)
                {
                    var appSettingSection = (DeploymentSection)group.Sections["deployment"];
                    if (appSettingSection.Retail)
                    {
                        return false;
                    }
                }                
                return true;
            }
            return false;
        }

        #region IDebugStatusReader Members


        public void ForceDebug()
        {
            forceDebug = true;
        }

        public void ForceRelease()
        {
            forceRelease = true;
        }

        #endregion
    }
}