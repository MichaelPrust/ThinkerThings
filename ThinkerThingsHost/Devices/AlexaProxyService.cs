using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Common;
using ThinkerThings.SignalRInterfaces;
using ThinkerThingsHost.Interfaces;
using ThinkerThingsHost.SignalRHost;

namespace ThinkerThingsHost.Devices
{
    internal class AlexaProxyService : IAlexaProxyService
    {
        private readonly IDevicesStateProvider _devicesStateProvider;
        private readonly IHubContext<SignalRHostImplementation, IThinkerThingsSignalRClient> _thinkerThingsSignalRHubContext;

        public AlexaProxyService(IDevicesStateProvider devicesStateProvider, IHubContext<SignalRHostImplementation, IThinkerThingsSignalRClient> thinkerThingsSignalRHubContext)
        {
            _devicesStateProvider = devicesStateProvider;
            _thinkerThingsSignalRHubContext = thinkerThingsSignalRHubContext;
        }

        public IDevicePortState GetStateOfPort(string portName)
        {
            var result = _devicesStateProvider.GetStateOfPort(portName);
            return result;
        }

        public string[] GetAllPorts(PortTypes portType)
        {
            return _devicesStateProvider.GetAllDevices().SelectMany(p => p.Ports.Where(x => x.PortType == portType).Select(x => x.PortName)).ToArray();
        }

        public bool ActivatePort(string portName)
        {
            var port = _devicesStateProvider.GetStateOfPort(portName);
            if (port != null && port.PortType != PortTypes.Switch)
                throw new InvalidOperationException($"O dispositivo '{portName}' não suporta esta operação");

            if (port == null || port.PortState == PortStates.Actived)
                return false;

            AlterarEstadoDaPorta(portName, PortStates.Actived, port);
            return true;
        }

        public bool DeactivatePort(string portName)
        {
            var port = _devicesStateProvider.GetStateOfPort(portName);
            if (port != null && port.PortType != PortTypes.Switch)
                throw new InvalidOperationException($"O dispositivo '{portName}' não suporta esta operação");

            if (port == null || port.PortState == PortStates.Deactived)
                return false;

            AlterarEstadoDaPorta(portName, PortStates.Deactived, port);
            return true;
        }

        public bool PulsePort(string portName)
        {
            var port = _devicesStateProvider.GetStateOfPort(portName);
            if (port != null && port.PortType != PortTypes.Pulse)
                throw new InvalidOperationException($"O dispositivo '{portName}' não suporta esta operação");

            if (port == null || port.PortState != port.DefaultPortState)
                return false;

            switch (port.DefaultPortState)
            {
                case PortStates.Actived:
                    AlterarEstadoDaPorta(portName, PortStates.Deactived, port);
                    break;
                case PortStates.Deactived:
                    AlterarEstadoDaPorta(portName, PortStates.Actived, port);
                    break;
                default:
                    throw new InvalidOperationException($"O estado '{port.DefaultPortState}' não possui um oposto para realizar a ativação");
            }

            return true;
        }

        private string AlterarEstadoDaPorta(string portName, PortStates targetPortState, IDevicePortState portState)
        {
            string result;
            var changePortStateContex = new ChangePortStateContext()
            {
                RequestId = "",
                SetState = targetPortState,
                TargetPortName = portName
            };
            _thinkerThingsSignalRHubContext.Clients.Client(portState.ConnectionId).ChangePortState(changePortStateContex);
            switch (targetPortState)
            {
                case PortStates.Actived:
                    result = $"{portName} será ligado";
                    break;
                case PortStates.Deactived:
                    result = $"{portName} será desligado";
                    break;
                default:
                    result = $"Não estou certo do que acontecerá com {portName}";
                    break;
            }

            return result;
        }

    }

}
