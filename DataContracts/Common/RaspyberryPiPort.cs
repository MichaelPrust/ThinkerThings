using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ThinkerThings.DataContracts.Common
{
    [DataContract]
    public class RaspyberryPiPort : DevicePort
    {
        [DataMember] public int PinId { get; set; }
    }
}
