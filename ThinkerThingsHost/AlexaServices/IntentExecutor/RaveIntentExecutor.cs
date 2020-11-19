using AlexaSkillsKit.Slu;
using System;
using System.Threading;
using ThinkerThings.DataContracts.Common;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal class RaveIntentExecutor : IIntentExecutor
    {
        public IntentExecutorContext Execute(IntentExecutorContext intentExecutorContext, Intent intent, IAlexaProxyService alexaProxyService)
        {
            var modoRave = new ModoRave(alexaProxyService);
            var thread = new Thread(() => modoRave.Execute());
            thread.Start();

            return new IntentExecutorContext()
            {
                Response = "Vamos começar a rave!",
                NeedConfirmation = false,
            };

        }

        private class ModoRave
        {
            private readonly IAlexaProxyService _alexaProxyService;
            private readonly string[] _ports;

            public ModoRave(IAlexaProxyService alexaProxyService)
            {
                _alexaProxyService = alexaProxyService;
                _ports = _alexaProxyService.GetAllPorts(PortTypes.Switch);
            }

            public void Execute()
            {
                var random = new Random();

                while (true)
                {
                    foreach (var port in _ports)
                    {
                        var i = random.Next(1, 1000);
                        if ((i & 1) == 1)
                        {
                            _alexaProxyService.ActivatePort(port);
                        }
                        else
                        {
                            _alexaProxyService.DeactivatePort(port);
                        }
                    }
                    Thread.Sleep(random.Next(200, 700));
                }
            }
        }
    }
}
