using System.Net.Http;

namespace ThinkerThingsHost.Interfaces
{
    internal interface IThinkerThingsSpeechlet
    {
        HttpResponseMessage GetResponse(HttpRequestMessage httpRequest);
    }
}
