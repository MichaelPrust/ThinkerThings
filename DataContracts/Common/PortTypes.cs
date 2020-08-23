using System.Runtime.Serialization;

namespace ThinkerThings.DataContracts.Common
{
    [DataContract]
    public enum PortTypes
    {
        None = 0,
        Switch,
        Pulse
    }

}
