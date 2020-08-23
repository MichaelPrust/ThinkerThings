using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices
{
    internal class ThinkerThingsAlexaServiceImplementation : IThinkerThingsAlexaService
    {
        private readonly IThinkerThingsSpeechlet _thinkerThingsSpeechlet;

        public ThinkerThingsAlexaServiceImplementation(IThinkerThingsSpeechlet thinkerThingsSpeechlet)
        {
            _thinkerThingsSpeechlet = thinkerThingsSpeechlet;
        }

        public IActionResult GetResponse(ControllerBase controllerBase)
        {
            var request = RequestTranscriptHelpers.ToHttpRequestMessage(controllerBase.Request);
            var result = _thinkerThingsSpeechlet.GetResponse(request);

            controllerBase.HttpContext.Response.RegisterForDispose(result);

            return new HttpResponseMessageResult(result);
        }

        public class HttpResponseMessageResult : IActionResult
        {
            private readonly HttpResponseMessage _responseMessage;

            public HttpResponseMessageResult(HttpResponseMessage responseMessage)
            {
                _responseMessage = responseMessage; // could add throw if null
            }

            public async Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.StatusCode = (int)_responseMessage.StatusCode;

                foreach (var header in _responseMessage.Headers)
                {
                    context.HttpContext.Response.Headers.TryAdd(header.Key, new StringValues(header.Value.ToArray()));
                }

                using (var stream = await _responseMessage.Content.ReadAsStreamAsync())
                {
                    await stream.CopyToAsync(context.HttpContext.Response.Body);
                    await context.HttpContext.Response.Body.FlushAsync();
                }
            }
        }
    }
}
