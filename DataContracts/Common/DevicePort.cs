﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ThinkerThings.DataContracts.Common
{
    [DataContract]
    public class DevicePort
    {
        [DataMember] public string PortName { get; set; }
        [DataMember] public PortTypes PortType { get; set; }
        [DataMember] public PortStates PortState { get; set; }
        [DataMember] public PortStates DefaultPortState { get; set; }
        [DataMember] public int PulseLength { get; set; }
    }
}