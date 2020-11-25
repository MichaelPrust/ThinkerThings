using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsHost.Interfaces
{
    internal interface IAlexaProxyService
    {
        IDevicePortState GetStateOfPort(string portName);
        bool ActivatePort(string portName);
        bool DeactivatePort(string portName);
        bool PulsePort(string portName);
        string[] GetAllPorts(PortTypes portType);
    }
}
