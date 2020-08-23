using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsHost.Interfaces
{
    public interface IDevicePortState
    {
        PortStates PortState { get; }
        public string DeviceName { get; }
        public string ConnectionId { get; }
    }
}
