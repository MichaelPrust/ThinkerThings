using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThinkerThingsHost.Interfaces
{
    internal interface IAlexaProxyService
    {
        string GetStatusMessage(string portName);
        string SetState(string portName, string intentName);
    }
}
