using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsHost.Interfaces
{
    internal interface IAlexaProxyService
    {
        string GetStatusMessage(string portName);
        string SetState(string portName, string intentName);
        bool ActivatePort(string portName);
        bool DeactivatePort(string portName);
        bool PulsePort(string portName);
        string[] GetAllPorts(PortTypes portType);
    }
}
