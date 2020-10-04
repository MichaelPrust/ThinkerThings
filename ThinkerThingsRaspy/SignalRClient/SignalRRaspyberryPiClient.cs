using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Devices;
using ThinkerThings.SignalRInterfaces;
using ThinkerThingsRaspy.PinManager;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.System.Threading;

namespace ThinkerThingsRaspy.SignalRClient
{
    internal class SignalRRaspyberryPiClient : SignalRClientBase, IThinkerThingsSignalRClient
    {
        private SignalRRaspyberryPiClient() : base()
        {
        }

        public static SignalRRaspyberryPiClient Instance { get; private set; }
        protected override string DeviceName => DeviceConfigurationService.Current.DeviceName;

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new SignalRRaspyberryPiClient();
                Instance.Configure();

                var thread = new Thread(() => Instance.StartConnection());
                thread.Start();
            }
        }

        public async Task ChangePortState(ChangePortStateContext changePortStateContext)
        {
            var context = changePortStateContext;
            await Task.Run(() =>
            {
                RaspyPinManager.Instance.ChangePortState(context);
            });

            UpdatePortState(changePortStateContext);
        }

        public void UpdatePortState(ChangePortStateContext changePortStateContext)
        {
            _thinkerThingsHubConnection.UpdatePortState(changePortStateContext);
        }

        public async Task InitializeRaspberryPi(RaspyberryPiConfiguration raspyberryPiConfiguration)
        {
            var configuration = raspyberryPiConfiguration;
            await Task.Run(() => DeviceConfigurationService.Current.SetRaspberryPiConfiguration(configuration));
        }

        protected override void ConfigureHubConnection(HubConnection hubConnection)
        {
            MapMethods<IThinkerThingsSignalRClient>(hubConnection, this);
        }

    }

}
