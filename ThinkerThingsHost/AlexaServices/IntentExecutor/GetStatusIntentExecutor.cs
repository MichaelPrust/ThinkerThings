using AlexaSkillsKit.Slu;
using System.Collections.Generic;
using System.Linq;
using ThinkerThings.DataContracts.Common;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal class GetStatusIntentExecutor : IIntentExecutor
    {
        public IntentExecutorContext Execute(IntentExecutorContext intentExecutorContext, Intent intent, IAlexaProxyService alexaProxyService)
        {
            intentExecutorContext = EnsureContexto(intentExecutorContext, intent);

            LocateMissingPorts(intentExecutorContext, alexaProxyService);

            var portStatesMessages = intentExecutorContext.TargetPorts.Select(p => GetStatusMessageForPort(p, alexaProxyService));

            IntentExecutorContext result;
            if (portStatesMessages.Any())
            {
                result = new IntentExecutorContext()
                {
                    NeedConfirmation = false,
                    Response = BuildResponseMessage(portStatesMessages)
                };
            }
            else
            {
                result = new IntentExecutorContext()
                {
                    NeedConfirmation = false,
                    Response = "Não foi possível determinar os dispositivos requisitados"
                };
            }
            return result;
        }

        private static IntentExecutorContext EnsureContexto(IntentExecutorContext intentExecutorContext, Intent intent)
        {
            if (intentExecutorContext == null)
            {
                intentExecutorContext = new IntentExecutorContext()
                {
                    Intent = intent.Name,
                    PortsQuery = PortsQueryProcessor.GetQueryFromIntent(intent)
                };
            }

            return intentExecutorContext;
        }

        private void LocateMissingPorts(IntentExecutorContext intentExecutorContext, IAlexaProxyService alexaProxyService)
        {
            if (intentExecutorContext.TargetPorts == null)
            {
                LocatePorts(intentExecutorContext, alexaProxyService);
            }
        }

        private void LocatePorts(IntentExecutorContext intentExecutorContext, IAlexaProxyService alexaProxyService)
        {
            var query = intentExecutorContext.PortsQuery;
            var ports = alexaProxyService.GetAllPorts(PortTypes.Switch);

            var querys = query.Split(" e ");
            var portInfos = querys.Select(p => PortInfo.Locate(ports, p)).ToArray();
            intentExecutorContext.TargetPorts = portInfos.Select(p => p.TargetPort).ToArray();
        }

        private string GetStatusMessageForPort(string portName, IAlexaProxyService alexaProxyService)
        {
            var portState = alexaProxyService.GetStateOfPort(portName);

            string result;
            if (portState != null)
            {
                switch (portState.PortType)
                {
                    case PortTypes.Switch:
                        result = GetStatusMessageForSwitchPort(portState, portName);
                        break;
                    case PortTypes.Pulse:
                        result = GetStatusMessageForPulsePort(portState, portName);
                        break;
                    default:
                        result = $"Não foi possível encontrar {portName}";
                        break;
                }
            }
            else
            {
                result = $"Não foi possível encontrar {portName}";
            }
            return result;
        }

        private string GetStatusMessageForSwitchPort(IDevicePortState portState, string portName)
        {
            string result;
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
            return result;
        }

        private string GetStatusMessageForPulsePort(IDevicePortState portState, string portName)
        {
            string result;
            if (portState.PortState != portState.DefaultPortState)
            {
                result = $"{portName} está ativo";
            }
            else
            {
                result = $"{portName} está em repouso";
            }

            return result;
        }

        private string BuildResponseMessage(IEnumerable<string> portStatesMessages)
        {
            var portsString = string.Join(", ", portStatesMessages);
            var result = $"A situação das portas requisitadas é: {portsString}";
            return result;
        }

    }

}
