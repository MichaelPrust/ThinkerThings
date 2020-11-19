using System.Runtime.Serialization;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThings.DataContracts.Devices
{
    [DataContract]
    public class RaspyberryPiConfiguration : GenericConfiguration
    {
        [DataMember] public RaspyberryPiPort[] RaspyberryPiPorts { get; set; }
    }

}
