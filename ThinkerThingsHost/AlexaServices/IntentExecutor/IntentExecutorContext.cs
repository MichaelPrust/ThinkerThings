using AlexaSkillsKit.Speechlet;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsHost.AlexaServices.IntentExecutor
{
    [DataContract]
    internal class IntentExecutorContext
    {
        public const string JsonAttributeKey = "thinkerThingsContext";

        [DataMember] public string PortsQuery { get; set; }
        [DataMember] public string Intent { get; set; }
        [DataMember] public string Response { get; set; }
        [DataMember] public string[] TargetPorts { get; set; }
        [DataMember] public bool NeedConfirmation { get; set; }
        [DataMember] public PortStates? TargetPortState { get; set; }

        public static IntentExecutorContext FromSession(Session session)
        {
            string data;
            if (!session.Attributes.TryGetValue(JsonAttributeKey, out data))
                return null;
            var result = JsonConvert.DeserializeObject<IntentExecutorContext>(data);
            return result;
        }

        public void PersistInSession(Session session)
        {
            string data = JsonConvert.SerializeObject(this);
            if (session.Attributes.ContainsKey(JsonAttributeKey))
            {
                session.Attributes[JsonAttributeKey] = data;
            }
            else
            {
                session.Attributes.Add(JsonAttributeKey, data);
            }
        }
    }
}
