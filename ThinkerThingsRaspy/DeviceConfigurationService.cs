using ThinkerThings.DataContracts.Devices;
using ThinkerThingsRaspy.PinManager;
using ThinkerThingsRaspy.SignalRClient;

namespace ThinkerThingsRaspy
{
    internal class DeviceConfigurationService
    {
        private DeviceConfigurationService()
        {
            DeviceName = "TesteRaspy";     
        }

        public static DeviceConfigurationService Current { get; } = new DeviceConfigurationService();

        public string DeviceName { get; }

        public void SetRaspberryPiConfiguration(RaspyberryPiConfiguration raspyberryPiConfiguration)
        {
            RaspyPinManager.Instance.LoadConfiguration(raspyberryPiConfiguration.RaspyberryPiPorts);
        }

        public GenericConfiguration GetGenericConfiguration()
        {
            var ports = RaspyPinManager.Instance.GetDevicePorts();

            var result = new GenericConfiguration()
            {
                DeviceName = DeviceName,
                ConnectionId = SignalRRaspyberryPiClient.Instance.ConnectionId,
                Ports = ports
            };

            return result;
        }

    }

}