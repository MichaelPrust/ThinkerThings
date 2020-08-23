using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Devices;

namespace ThinkerThings.SignalRInterfaces
{
    public interface IThinkerThingsSignalRClient
    {
        Task InitializeRaspberryPi(RaspyberryPiConfiguration raspyberryPiConfiguration);
        Task ChangePortState(ChangePortStateContext changePortStateContext);
    }

}
