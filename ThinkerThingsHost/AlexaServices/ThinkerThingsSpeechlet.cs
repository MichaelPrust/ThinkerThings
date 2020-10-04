using AlexaSkillsKit.Slu;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThinkerThingsHost.AlexaServices.IntentExecutor;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices
{
    internal class ThinkerThingsSpeechlet : SpeechletBase, ISpeechletWithContext, IThinkerThingsSpeechlet
    {

        private readonly IAlexaProxyService _alexaProxyService;

        private const string PortNameSlotName = "portName";

        public ThinkerThingsSpeechlet(IAlexaProxyService alexaProxyService)
        {
            _alexaProxyService = alexaProxyService;
        }

        public SpeechletResponse OnLaunch(LaunchRequest launchRequest, Session session, Context context)
        {
            string speechOutput = "Bem vindo ao Coisas Pensantes, o centro de controle da sua casa.";

            return BuildSpeechletResponse("Welcome", speechOutput, false);
        }

        public SpeechletResponse OnIntent(IntentRequest intentRequest, Session session, Context context)
        {
            Intent intent = intentRequest.Intent;

            var intentExecutorContext = IntentExecutorContext.FromSession(session);

            var intentExecutor = IntentExecutorLocator.GetExecutorForIntent(intent.Name);
            var result = intentExecutor.Execute(intentExecutorContext, intent, _alexaProxyService);

            if (result.NeedConfirmation)
            {
                result.PersistInSession(session);
            }
            else if (intentExecutorContext != null)
            {
                session.Attributes.Remove(IntentExecutorContext.JsonAttributeKey);
            }

            return BuildSpeechletResponse("Resposta", result.Response, false);
        }

        public void OnSessionEnded(SessionEndedRequest sessionEndedRequest, Session session, Context context)
        {

        }

        public void OnSessionStarted(SessionStartedRequest sessionStartedRequest, Session session, Context context)
        {

        }

        private SpeechletResponse GetStatus(Intent intent, Session session)
        {
            IDictionary<string, Slot> slots = intent.Slots;

            Slot portNameSlot = slots[PortNameSlotName];
            string speechOutput;
            if (portNameSlot != null)
            {
                speechOutput = _alexaProxyService.GetStatusMessage(portNameSlot.Value);

            }
            else
            {
                speechOutput = "Não foi possível determinar o dispositivo";
            }

            var result = BuildSpeechletResponse("Status", speechOutput, false);
            return result;
        }

        private SpeechletResponse SetStatus(Intent intent, Session session)
        {
            IDictionary<string, Slot> slots = intent.Slots;

            Slot portNameSlot = slots[PortNameSlotName];
            string speechOutput;
            if (portNameSlot != null)
            {
                speechOutput = _alexaProxyService.SetState(portNameSlot.Value, intent.Name);

            }
            else
            {
                speechOutput = "Não foi possível determinar o dispositivo";
            }

            var result = BuildSpeechletResponse("Set status", speechOutput, false);
            return result;
        }

        private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = String.Format("SessionSpeechlet - {0}", title);
            card.Content = String.Format("SessionSpeechlet - {0}", output);

            // Create the plain text output.
            PlainTextOutputSpeech speech = new PlainTextOutputSpeech();
            speech.Text = output;

            // Create the speechlet response.
            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;
            response.OutputSpeech = speech;
            response.Card = card;

            return response;
        }

    }

}
