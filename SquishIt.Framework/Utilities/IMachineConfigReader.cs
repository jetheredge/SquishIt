namespace SquishIt.Framework.Utilities
{
    public interface IMachineConfigReader
    {
        bool IsNotRetailDeployment { get; }
    }
}