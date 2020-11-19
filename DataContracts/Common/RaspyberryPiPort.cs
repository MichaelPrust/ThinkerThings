using System.Runtime.Serialization;

namespace ThinkerThings.DataContracts.Common
{
    [DataContract]
    public class RaspyberryPiPort : DevicePort
    {
        [DataMember] public int PinId { get; set; }
    }
}
