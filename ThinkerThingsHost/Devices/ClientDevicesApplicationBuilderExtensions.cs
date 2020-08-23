using Microsoft.Extensions.DependencyInjection;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.Devices
{
    public static class ClientDevicesApplicationBuilderExtensions
    {
        public static void AddClientDevices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IDevicesStateProvider), typeof(DevicesStateProvider));
            services.AddTransient(typeof(IDevicesConfigurationProvider), typeof(DevicesConfigurationProvider));
            services.AddTransient(typeof(IAlexaProxyService), typeof(AlexaProxyService));

        }

    }

}
