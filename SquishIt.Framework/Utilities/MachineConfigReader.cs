using System.Configuration;
using System.Web.Configuration;

namespace SquishIt.Framework.Utilities
{
    public interface IMachineConfigReader
    {
        bool IsNotRetailDeployment { get; }
    }

    class MachineConfigReader : IMachineConfigReader
    {
        public bool IsNotRetailDeployment
        {
            get
            {
                //check retail setting in machine.config
                //Thanks Dave Ward! http://www.encosia.com
                System.Configuration.Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
                var group = machineConfig.GetSectionGroup("system.web");
                if(group != null)
                {
                    var appSettingSection = (DeploymentSection)group.Sections["deployment"];
                    if(appSettingSection.Retail)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}