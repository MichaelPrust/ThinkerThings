using System;
using System.Collections.Generic;
using System.Text;
using ThinkerThings.DataContracts.Commands;

namespace ThinkerThings.SignalRInterfaces
{
    public interface IThinkerThingsSignalRHost
    {
        void RequireConfiguration(string deviceName);
        void UpdatePortState(ChangePortStateContext changePortStateContext);
    }

}
