using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Common;
using ThinkerThings.SignalRInterfaces;
using ThinkerThingsHost.AlexaServices;
using ThinkerThingsHost.Interfaces;
using ThinkerThingsHost.SignalRHost;

namespace ThinkerThingsHost.Devices
{
    internal class AlexaProxyService : IAlexaProxyService
    {
        private static readonly string[] ActivateIntents = new string[]
        {
            AlexaIntents.ActivateIntent.Name
        };

        private static readonly string[] DeactivateIntents = new string[]
        {
            AlexaIntents.DeactivateIntent.Name
        };

        private readonly IDevicesStateProvider _devicesStateProvider;
        private readonly IHubContext<SignalRHostImplementation, IThinkerThingsSignalRClient> _thinkerThingsSignalRHubContext;

        private readonly List<Tuple<string, string>> _sinonimos = new List<Tuple<string, string>>()
        {
            Tuple.Create("4.º", "quarto"),
        };

        public AlexaProxyService(IDevicesStateProvider devicesStateProvider, IHubContext<SignalRHostImplementation, IThinkerThingsSignalRClient> thinkerThingsSignalRHubContext)
        {
            _devicesStateProvider = devicesStateProvider;
            _thinkerThingsSignalRHubContext = thinkerThingsSignalRHubContext;
        }

        public string GetStatusMessage(string portName)
        {
            portName = TraduzirSinonimosDaMensagem(portName);
            var portState = _devicesStateProvider.GetStateOfPort(portName);

            string result;
            if (portState != null)
            {
                switch (portState.PortState)
                {
                    case PortStates.None:
                        result = $"{portName} não está configurada";
                        break;
                    case PortStates.Actived:
                        result = $"{portName} está ligada";
                        break;
                    case PortStates.Deactived:
                        result = $"{portName} está desligada";
                        break;
                    default:
                        result = $"O estatus de {portName} é {portState.PortState} e não há suporte para ele";
                        break;
                }
            }
            else
            {
                result = $"Não foi possível encontrar {portName}";
            }
            return result;
        }

        public string SetState(string portName, string intentName)
        {
            portName = TraduzirSinonimosDaMensagem(portName);
            string result;
            PortStates targetPortState;

            var comparer = StringComparer.InvariantCultureIgnoreCase;
            if (ActivateIntents.Contains(intentName, comparer))
            {
                targetPortState = PortStates.Actived;
            }
            else if (DeactivateIntents.Contains(intentName, comparer))
            {
                targetPortState = PortStates.Deactived;
            }
            else
            {
                targetPortState = PortStates.None;
            }

            result = ObterRespostaDeAcordoComOEstadoRequisitado(portName, intentName, targetPortState);

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

        private string TraduzirSinonimosDaMensagem(string message)
        {
            string result = message;
            foreach (var sinonimo in _sinonimos)
            {
                result = result.Replace(sinonimo.Item1, sinonimo.Item2);
            }
            return result;
        }

        private string ObterRespostaDeAcordoComOEstadoRequisitado(string portName, string intentName, PortStates targetPortState)
        {
            string result;
            var portState = _devicesStateProvider.GetStateOfPort(portName);
            if (targetPortState != PortStates.None)
            {
                if (portState.PortState == targetPortState)
                {
                    result = InformarQueJaSeEncontraNoEstadoRequisitado(portName, targetPortState);
                }
                else
                {
                    result = AlterarEstadoDaPorta(portName, targetPortState, portState);
                }
            }
            else
            {
                result = $"Não há suporte para a intenção {intentName}";
            }

            return result;
        }

        private static string InformarQueJaSeEncontraNoEstadoRequisitado(string portName, PortStates targetPortState)
        {
            string result;
            switch (targetPortState)
            {
                case PortStates.Actived:
                    result = $"{portName} já se encontra ligado";
                    break;
                case PortStates.Deactived:
                    result = $"{portName} já se encontra desligado";
                    break;
                default:
                    result = $"O estatus de {portName} é {targetPortState} e não há suporte para ele";
                    break;
            }

            return result;
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
