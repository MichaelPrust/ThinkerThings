﻿using AlexaSkillsKit.Slu;
using System.Linq;
using System.Text;
using ThinkerThings.DataContracts.Common;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal class PulseIntentExecutor : IIntentExecutor
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

                foreach (var changePortStateRequest in changePortStateRequests)
                {
                    changePortStateRequest.StateChanged = alexaProxyService.PulsePort(changePortStateRequest.PortName);
                }

                intentExecutorContext.Response = BuildResponseMessage(changePortStateRequests);
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
                    PortsQuery = PortsQueryProcessor.GetQueryFromIntent(intent)
                };
            }

            return intentExecutorContext;
        }

        private void LocatePorts(IntentExecutorContext intentExecutorContext, IAlexaProxyService alexaProxyService)
        {
            var query = intentExecutorContext.PortsQuery;
            var ports = alexaProxyService.GetAllPorts(PortTypes.Pulse);

            var querys = query.Split(" e ");
            var portInfos = querys.Select(p => PortInfo.Locate(ports, p)).ToArray();
            intentExecutorContext.TargetPorts = portInfos.Select(p => p.TargetPort).ToArray();

            intentExecutorContext.NeedConfirmation = portInfos.Any(p => !p.Ideal);
        }

        private string BuildResponseMessage(ChangePortStateRequest[] changePortStateRequests)
        {
            string willWord = "será";
            string willWordPlus = "serão";
            string inCurrentState = "estava";
            string inCurrentStatePlus = "estavam";

            var changed = changePortStateRequests.Where(p => p.StateChanged).ToArray();
            var notChanged = changePortStateRequests.Where(p => !p.StateChanged).ToArray();

            string changedStateWord = changed.Length > 1 ? "ativados" : "ativado";
            string notChangedStateWord = notChanged.Length > 1 ? "ativados" : "ativado";

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
            string word = "ativar";

            var portsText = string.Join(" e ", intentExecutorContext.TargetPorts);
            var message = intentExecutorContext.TargetPorts.Length > 1 ? $"Deseja {word} os dispositivos {portsText} ?" : $"Deseja {word} o dispositivo {portsText} ?";
            return message;
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