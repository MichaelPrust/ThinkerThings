﻿using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsHost.Interfaces
{
    public interface IDevicePortState
    {
        PortTypes PortType { get; }
        PortStates PortState { get; }
        PortStates DefaultPortState { get; }
        public string DeviceName { get; }
        public string ConnectionId { get; }
    }
}
