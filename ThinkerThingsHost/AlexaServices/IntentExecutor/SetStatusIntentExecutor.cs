using AlexaSkillsKit.Slu;
using AlexaSkillsKit.Speechlet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Common;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal class SetStatusIntentExecutor : IIntentExecutor
    {
        public IntentExecutorContext Execute(IntentExecutorContext intentExecutorContext, Intent intent, IAlexaProxyService alexaProxyService)
        {
            intentExecutorContext = EnsureContexto(intentExecutorContext, intent);

            LocateMissingPorts(intentExecutorContext, alexaProxyService);

            if (intentExecutorContext.TargetPorts == null)
            {
                intentExecutorContext.Response = $"Não foi possível localizar o dispositivo {intentExecutorContext.PortsQuery}";
            }
            else if (!intentExecutorContext.NeedConfirmation)
            {
                var changePortStateRequests = intentExecutorContext.TargetPorts.Select(p => new ChangePortStateRequest(p)).ToArray();
                var targetState = GetTargetState(intentExecutorContext);
                Func<string, bool> changeStateFunc;
                switch (targetState)
                {
                    case TargetStates.Activate:
                        changeStateFunc = alexaProxyService.ActivatePort;
                        break;
                    case TargetStates.Deactivate:
                        changeStateFunc = alexaProxyService.DeactivatePort;
                        break;
                    default:
                        throw new SpeechletException($"TargetStates '{targetState}' não é suportado");
                }

                foreach (var changePortStateRequest in changePortStateRequests)
                {
                    changePortStateRequest.StateChanged = changeStateFunc(changePortStateRequest.PortName);
                }

                intentExecutorContext.Response = BuildResponseMessage(changePortStateRequests, targetState);
            }
            else if (intentExecutorContext.NeedConfirmation)
            {
                intentExecutorContext.Response = BuildConfirmationMessage(intentExecutorContext);
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

        private static IntentExecutorContext EnsureContexto(IntentExecutorContext intentExecutorContext, Intent intent)
        {
            if (intentExecutorContext == null)
            {
                intentExecutorContext = new IntentExecutorContext()
                {
                    Intent = intent.Name,
                    PortsQuery = PortsQueryProcessor.GetQueryFromIntent(intent),
                    TargetPortState = string.Equals(AlexaIntents.ActivateIntent.Name, intent.Name, StringComparison.InvariantCultureIgnoreCase) ?
                        PortStates.Actived : PortStates.Deactived
                };
            }

            return intentExecutorContext;
        }

        private void LocatePorts(IntentExecutorContext intentExecutorContext, IAlexaProxyService alexaProxyService)
        {
            var query = intentExecutorContext.PortsQuery;
            var ports = alexaProxyService.GetAllPorts(PortTypes.Switch);

            var querys = query.Split(" e ");
            var portInfos = querys.Select(p => PortInfo.Locate(ports, p)).ToArray();
            intentExecutorContext.TargetPorts = portInfos.Select(p => p.TargetPort).ToArray();

            intentExecutorContext.NeedConfirmation = portInfos.Any(p => !p.Ideal);

        }

        private TargetStates GetTargetState(IntentExecutorContext intentExecutorContext)
        {
            if (string.Equals(intentExecutorContext.Intent, AlexaIntents.ActivateIntent.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return TargetStates.Activate;
            }
            else if (string.Equals(intentExecutorContext.Intent, AlexaIntents.DeactivateIntent.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return TargetStates.Deactivate;
            }
            else
            {
                throw new SpeechletException($"Intent '{intentExecutorContext.Intent}' não é suportada");
            }
        }

        private string BuildResponseMessage(ChangePortStateRequest[] changePortStateRequests, TargetStates targetState)
        {
            string activatedWord = "ligado";
            string activatedWordPlus = "ligados";
            string deactivatedWord = "desligado";
            string deactivatedWordPlus = "desligados";
            string willWord = "será";
            string willWordPlus = "serão";
            string inCurrentState = "estava";
            string inCurrentStatePlus = "estavam";

            var changed = changePortStateRequests.Where(p => p.StateChanged).ToArray();
            var notChanged = changePortStateRequests.Where(p => !p.StateChanged).ToArray();

            string changedStateWord;
            string notChangedStateWord;
            switch (targetState)
            {
                case TargetStates.Activate:
                    changedStateWord = changed.Length > 1 ? activatedWordPlus : activatedWord;
                    notChangedStateWord = notChanged.Length > 1 ? activatedWordPlus : activatedWord;
                    break;
                case TargetStates.Deactivate:
                    changedStateWord = changed.Length > 1 ? deactivatedWordPlus : deactivatedWord;
                    notChangedStateWord = notChanged.Length > 1 ? deactivatedWordPlus : deactivatedWord;
                    break;
                default:
                    throw new SpeechletException($"TargetStates '{targetState}' não é suportado");
            }

            var sb = new StringBuilder();
            if (changed.Any())
            {
                var changedPorts = string.Join(", ", changed.Select(p => p.PortName));
                var changedMessage = $"{changedPorts} {(changed.Length > 1 ? willWordPlus : willWord)} {changedStateWord}. ";
                sb.Append(changedMessage);
            }
            if (notChanged.Any())
            {
                var changedPorts = string.Join(", ", notChanged.Select(p => p.PortName));
                var changedMessage = $"{changedPorts} já {(notChanged.Length > 1 ? inCurrentStatePlus : inCurrentState)} {notChangedStateWord}. ";
                sb.Append(changedMessage);
            }
            return sb.ToString();
        }

        private string BuildConfirmationMessage(IntentExecutorContext intentExecutorContext)
        {
            var activateWord = "ligar";
            var deactivateWord = "desligar";

            string word;
            switch (intentExecutorContext.TargetPortState)
            {
                case PortStates.Actived:
                    word = activateWord;
                    break;
                case PortStates.Deactived:
                    word = deactivateWord;
                    break;
                default:
                    throw new SpeechletException($"TargetStates '{intentExecutorContext.TargetPortState}' não é suportado");
            }

            var portsText = string.Join(" e ", intentExecutorContext.TargetPorts);
            var message = $"Deseja {word} os dispositivos {portsText} ?";
            return message;
        }

        private enum TargetStates
        {
            Activate,
            Deactivate
        }

        private class ChangePortStateRequest
        {
            public ChangePortStateRequest(string portName)
            {
                PortName = portName;
                StateChanged = false;
            }

            public string PortName { get; set; }
            public bool StateChanged { get; set; }
        }

    }

}