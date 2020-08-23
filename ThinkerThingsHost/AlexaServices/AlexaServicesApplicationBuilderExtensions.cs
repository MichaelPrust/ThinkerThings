using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
