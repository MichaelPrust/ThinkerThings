using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.SignalRInterfaces;

namespace ThinkerThingsRaspy.SignalRClient
{
    internal abstract class SignalRClientBase
    {
        protected readonly ThinkerThingsHubConnection _thinkerThingsHubConnection;
        private readonly HubConnection _hubConnection;

        private const string _baseUrl = "https://0efb800f04a1.ngrok.io/";

        protected SignalRClientBase()
        {
            var builder = new HubConnectionBuilder();
            builder.WithUrl($"{_baseUrl}raspySignalR");

            _hubConnection = builder.Build();

            _thinkerThingsHubConnection = new ThinkerThingsHubConnection(_hubConnection);
        }

        public string ConnectionId => _hubConnection.State == HubConnectionState.Connected ? _hubConnection.ConnectionId : string.Empty;

        protected void Configure()
        {
            ConfigureHubConnection(_hubConnection);
            _hubConnection.Closed += HubConnectionClosed;
        }

        protected async Task StartConnection()
        {
            while (_thinkerThingsHubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _thinkerThingsHubConnection.Connect();
                }
                catch(Exception)
                {
                    //não deve travar
                }
            }

            await Task.Delay(5000);
            _thinkerThingsHubConnection.RequireConfiguration(DeviceName);
        }

        private async Task HubConnectionClosed(Exception arg)
        {
            await StartConnection();
        }

        protected abstract void ConfigureHubConnection(HubConnection hubConnection);
        protected abstract string DeviceName { get; }

        protected static void MapMethods<T>(HubConnection hubConnection, T obj)
            where T: class
        {
            var type = typeof(T);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var onReceivedOneParameterBase = typeof(SignalRClientBase).GetMethod(nameof(OnReceivedOneParameter), BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var methodParameters = method.GetParameters();
                switch (methodParameters.Length)
                {
                    case 1:
                        var action = onReceivedOneParameterBase.MakeGenericMethod(methodParameters.First().ParameterType);
                        action.Invoke(null, GetObjectArray(hubConnection, method, obj));
                        break;
                    default:
                        throw new InvalidOperationException($"Não há suporte para métodos com '{methodParameters.Length}' parâmetros");
                }
            }
        }

        private static void OnReceivedOneParameter<T>(HubConnection hubConnection, MethodInfo methodInfo, object obj)
            where T: class
        {
            hubConnection.On<T>(methodInfo.Name, p => methodInfo.Invoke(obj, GetObjectArray(p)));
        }

        private static object[] GetObjectArray(params object[] objects)
        {
            return objects;
        }

        protected class ThinkerThingsHubConnection : IThinkerThingsSignalRHost
        {
            private readonly HubConnection _hubConnection;

            public HubConnectionState State => _hubConnection.State;

            public ThinkerThingsHubConnection(HubConnection hubConnection)
            {
                _hubConnection = hubConnection;
            }

            public void RequireConfiguration(string deviceName)
            {
                _hubConnection.SendCoreAsync(nameof(RequireConfiguration), GetObjectArray(deviceName));
            }

            public void UpdatePortState(ChangePortStateContext changePortStateContext)
            {
                _hubConnection.SendCoreAsync(nameof(UpdatePortState), GetObjectArray(changePortStateContext));
            }

            public async Task Connect()
            {
                await _hubConnection.StartAsync();
            }

        }

    }

}
