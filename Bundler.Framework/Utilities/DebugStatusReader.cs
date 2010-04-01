using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace Bundler.Framework.Utilities
{
    public class DebugStatusReader: IDebugStatusReader
    {
        public bool IsDebuggingEnabled()
        {
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
    }
}