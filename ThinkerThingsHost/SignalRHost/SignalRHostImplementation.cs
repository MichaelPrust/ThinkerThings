using Microsoft.AspNetCore.SignalR;
using System;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Devices;
using ThinkerThings.SignalRInterfaces;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.SignalRHost
{
    public class SignalRHostImplementation : Hub<IThinkerThingsSignalRClient>, IThinkerThingsSignalRHost
    {
        private readonly IDevicesConfigurationProvider _devicesConfigurationProvider;
        private readonly IDevicesStateProvider _devicesStateProvider;

        public SignalRHostImplementation(IDevicesConfigurationProvider devicesConfigurationProvider, IDevicesStateProvider devicesStateProvider)
        {
            _devicesConfigurationProvider = devicesConfigurationProvider;
            _devicesStateProvider = devicesStateProvider;
        }

        public void RequireConfiguration(string deviceName)
        {
            var currentState = _devicesStateProvider.GetDeviceByName(deviceName);
            var configuration = currentState ?? _devicesConfigurationProvider.GetConfigurationFor(deviceName);
            if (configuration != null)
            {
                if (configuration is RaspyberryPiConfiguration raspberryPiConfiguration)
                {
                    configuration.ConnectionId = Context.ConnectionId;
                    _devicesStateProvider.LoadDevice(configuration);
                    Clients.Caller.InitializeRaspberryPi(raspberryPiConfiguration);
                }
                else
                {
                    throw new InvalidOperationException($"Configuração '{configuration.GetType().FullName}' não é suportada");
                }
            }
        }

        public void UpdatePortState(ChangePortStateContext changePortStateContext)
        {
            var callerId = Context.ConnectionId;
            _devicesStateProvider.UpdatePortState(changePortStateContext, callerId);
        }

    }

}
