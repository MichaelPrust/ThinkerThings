using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ThinkerThings.DataContracts.Common
{
    [DataContract]
    public enum PortStates
    {
        None,
        Actived,
        Deactived
    }

}
