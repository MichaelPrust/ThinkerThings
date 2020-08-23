// Copyright 2018 Stefan Negritoiu (FreeBusy) and contributors. See LICENSE file for more information.

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;
using ThinkerThingsHost.AlexaServices;

namespace Sample.Controllers
{
    public class AlexaController : Controller
    {
        [Route("alexa/sample-session")]
        [HttpPost]
        public IActionResult SampleSession()
        {
            var speechlet = new SampleSessionSpeechlet();
            var request = RequestTranscriptHelpers.ToHttpRequestMessage(Request);
            var result = speechlet.GetResponse(request);

            this.HttpContext.Response.RegisterForDispose(result);

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
