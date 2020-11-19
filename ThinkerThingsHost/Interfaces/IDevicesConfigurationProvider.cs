using ThinkerThings.DataContracts.Devices;

namespace ThinkerThingsHost.Interfaces
{
    public interface IDevicesConfigurationProvider
    {
        GenericConfiguration GetConfigurationFor(string deviceName);
    }
}
