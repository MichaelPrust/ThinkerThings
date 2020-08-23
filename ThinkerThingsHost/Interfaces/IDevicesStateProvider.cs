using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Devices;

namespace ThinkerThingsHost.Interfaces
{
    public interface IDevicesStateProvider
    {
        IDevicePortState GetStateOfPort(string portName);
        void UpdatePortState(ChangePortStateContext changePortStateContext, string requesterId);
        void LoadDevice(GenericConfiguration genericConfiguration);
        GenericConfiguration GetDeviceByName(string deviceName);
        GenericConfiguration GetDeviceById(string id);
    }
}
