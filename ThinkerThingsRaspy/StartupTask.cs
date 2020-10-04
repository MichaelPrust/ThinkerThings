using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using ThinkerThingsRaspy.SignalRClient;
using ThinkerThingsRaspy.WebServer;
using Windows.System.Threading;
using System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ThinkerThingsRaspy
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            SignalRRaspyberryPiClient.Initialize();
            var webserver = new MyWebserver();
            var thread = new Thread(() => webserver.Start());
            thread.Start();

        }
    }
}
