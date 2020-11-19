using AlexaSkillsKit.Slu;
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