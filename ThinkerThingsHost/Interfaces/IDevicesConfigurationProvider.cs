using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Devices;

namespace ThinkerThingsHost.Interfaces
{
    public interface IDevicesConfigurationProvider
    {
        GenericConfiguration GetConfigurationFor(string deviceName);
    }
}
