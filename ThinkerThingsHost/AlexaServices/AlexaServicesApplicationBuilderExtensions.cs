using Microsoft.Extensions.DependencyInjection;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.AlexaServices
{
    public static class AlexaServicesApplicationBuilderExtensions
    {
        public static void AddAlexaServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IThinkerThingsAlexaService), typeof(ThinkerThingsAlexaServiceImplementation));
            services.AddTransient(typeof(IThinkerThingsSpeechlet), typeof(ThinkerThingsSpeechlet));

        }

    }
}
