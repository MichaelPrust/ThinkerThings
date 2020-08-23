using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using ThinkerThingsRaspy.SignalRClient;
using ThinkerThingsRaspy.WebServer;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ThinkerThingsRaspy
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            SignalRRaspyberryPiClient.Initialize();

            _deferral = taskInstance.GetDeferral();

            var webserver = new MyWebserver();

            await ThreadPool.RunAsync(workItem =>
            {                
                webserver.Start();
            });
        }
    }
}
