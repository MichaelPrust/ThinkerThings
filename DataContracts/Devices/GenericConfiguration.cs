using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThings.DataContracts.Devices
{
    [DataContract]
    public class GenericConfiguration
    {
        [DataMember] public string ConnectionId { get; set; }
        [DataMember] public string DeviceName { get; set; }
        [DataMember] public DevicePort[] Ports { get; set; }
    }

}
