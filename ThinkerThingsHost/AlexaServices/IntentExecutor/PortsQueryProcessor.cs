using AlexaSkillsKit.Slu;
using System;
using System.Collections.Generic;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    internal static class PortsQueryProcessor
    {
        private static readonly List<Tuple<string, string>> _sinonimos = new List<Tuple<string, string>>()
        {
            Tuple.Create("4.º", "quarto"),
            Tuple.Create("4º", "quarto"),
        };

        public static string GetQueryFromIntent(Intent intent)
        {
            var result = intent.Slots[AlexaIntents.ActivateIntent.Fields.PortNameSlotName].Value;
            result = SubstituirSinonimos(result);

            return result;
        }

        private static string SubstituirSinonimos(string message)
        {
            string result = message;
            foreach (var sinonimo in _sinonimos)
            {
                result = result.Replace(sinonimo.Item1, sinonimo.Item2);
            }
            return result;
        }
    }
}
