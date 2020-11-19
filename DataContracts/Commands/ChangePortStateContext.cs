using System.Runtime.Serialization;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThings.DataContracts.Commands
{
    [DataContract]
    public class ChangePortStateContext
    {
        [DataMember] public string RequestId { get; set; }
        [DataMember] public string TargetPortName { get; set; }
        [DataMember] public PortStates SetState { get; set; }
    }

}
