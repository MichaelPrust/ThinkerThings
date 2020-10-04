using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Common;
using ThinkerThings.DataContracts.Devices;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.Devices
{
    public class DevicesConfigurationProvider : IDevicesConfigurationProvider
    {
        public GenericConfiguration GetConfigurationFor(string deviceName)
        {
            if (string.Equals(deviceName, "TesteRaspy", StringComparison.InvariantCultureIgnoreCase))
                return TestConfiguration();

            return null;
        }

        private GenericConfiguration TestConfiguration()
        {
            var ports = TestRaspberryPiPorts();
            var result = new RaspyberryPiConfiguration()
            {
                DeviceName = "TesteRaspy",
                Ports = ports,
                RaspyberryPiPorts = ports
            };

            return result;
        }

        private RaspyberryPiPort[] TestRaspberryPiPorts()
        {
            return new RaspyberryPiPort[]
            {
                new RaspyberryPiPort()
                {
                    PinId = 16,
                    PortName = "portão",
                    PortState = PortStates.Actived,
                    PortType = PortTypes.Pulse,
                    DefaultPortState = PortStates.Actived,
                    PulseLength = 5000
                },
                new RaspyberryPiPort()
                {
                    PinId = 20,
                    PortName = "luz do quarto",
                    PortState = PortStates.Deactived,
                    PortType = PortTypes.Switch
                },
                new RaspyberryPiPort()
                {
                    PinId = 21,
                    PortName = "luz da sala",
                    PortState = PortStates.Actived,
                    PortType = PortTypes.Switch
                },
            };
        }

    }

}
