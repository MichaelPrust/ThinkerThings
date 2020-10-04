using AlexaSkillsKit.Slu;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal interface IIntentExecutor
    {
        IntentExecutorContext Execute(IntentExecutorContext intentExecutorContext, Intent intent, IAlexaProxyService alexaProxyService);
    }
}
