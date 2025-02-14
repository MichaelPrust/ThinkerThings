﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using ThinkerThingsRaspy.PinManager;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ThinkerThingsRaspy.WebServer
{
    internal class MyWebserver
    {
        private const uint BufferSize = 8192;

        public async void Start()
        {
            var listener = new StreamSocketListener();

            await listener.BindServiceNameAsync("8085");

            listener.ConnectionReceived += async (sender, args) =>
            {
                var request = new StringBuilder();

                using (var input = args.Socket.InputStream)
                {
                    var data = new byte[BufferSize];
                    IBuffer buffer = data.AsBuffer();
                    var dataRead = BufferSize;

                    while (dataRead == BufferSize)
                    {
                        await input.ReadAsync(
                             buffer, BufferSize, InputStreamOptions.Partial);
                        request.Append(Encoding.UTF8.GetString(
                                                      data, 0, data.Length));
                        dataRead = buffer.Length;
                    }
                }

                string pinStatus = GetPinStatus();

                using (var output = args.Socket.OutputStream)
                {
                    using (var response = output.AsStreamForWrite())
                    {
                        var html = Encoding.UTF8.GetBytes(
                        $"<html><head><title>Background Message</title></head><body>Hello from the background process!<br/>{pinStatus}</body></html>");
                        using (var bodyStream = new MemoryStream(html))
                        {
                            var header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\n\r\n";
                            var headerArray = Encoding.UTF8.GetBytes(header);
                            await response.WriteAsync(headerArray,
                                                      0, headerArray.Length);
                            await bodyStream.CopyToAsync(response);
                            await response.FlushAsync();
                        }
                    }
                }
            };
        }

        private string GetPinStatus()
        {
            var configuration = DeviceConfigurationService.Current.GetGenericConfiguration();
            if (configuration != null)
            {
                var sb = new StringBuilder();
                sb.Append($"<br/>ConnectionID: {configuration.ConnectionId}");
                sb.Append($"<br/>DeviceName: {configuration.DeviceName}");
                sb.Append($"<br/>Ports configured: {configuration.Ports?.Length}");
                foreach (var port in configuration.Ports)
                {
                    sb.Append($"<br/>[Port: {port.PortName}]");
                    sb.Append($"<br/>State: {port.PortState}");
                }

                return sb.ToString();
            }
            return string.Empty;
            
        }

        private static string GetQuery(StringBuilder request)
        {
            var requestLines = request.ToString().Split(' ');

            var url = requestLines.Length > 1
                              ? requestLines[1] : string.Empty;

            var uri = new Uri("http://localhost" + url);
            var query = uri.Query;
            return query;
        }

    }

}
