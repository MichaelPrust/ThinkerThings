using AlexaSkillsKit.Slu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal class YesIntentExecutor : IIntentExecutor
    {
        public IntentExecutorContext Execute(IntentExecutorContext intentExecutorContext, Intent intent, IAlexaProxyService alexaProxyService)
        {
            var intentExecutor = IntentExecutorLocator.GetExecutorForIntent(intentExecutorContext.Intent);
            intentExecutorContext.NeedConfirmation = false;

            return intentExecutor.Execute(intentExecutorContext, intent, alexaProxyService);
        }

    }

}