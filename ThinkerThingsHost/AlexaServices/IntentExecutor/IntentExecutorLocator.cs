using AlexaSkillsKit.Speechlet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal static class IntentExecutorLocator
    {
        private static readonly string[] SetStatusIntents = new string[]
        {
            AlexaIntents.ActivateIntent.Name,
            AlexaIntents.DeactivateIntent.Name,
        };

        public static IIntentExecutor GetExecutorForIntent(string intentName)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;

            if (IsIntet(AlexaIntents.GetStatusIntent.Name, intentName))
            {
                return new GetStatusIntentExecutor();
            }
            else if (SetStatusIntents.Contains(intentName, comparer))
            {
                return new SetStatusIntentExecutor();
            }
            else if (IsIntet(AlexaIntents.YesIntent.Name, intentName))
            {
                return new YesIntentExecutor();
            }
            else if (IsIntet(AlexaIntents.PulseIntent.Name, intentName))
            {
                return new PulseIntentExecutor();
            }
            else if (IsIntet("modoRaveIntent", intentName))
            {
                return new RaveIntentExecutor();
            }
            else
            {
                throw new SpeechletException("Invalid Intent");
            }
        }

        private static bool IsIntet(string expectedIntent, string intentName)
        {
            return string.Equals(expectedIntent, intentName, StringComparison.InvariantCultureIgnoreCase);
        }

    }

}