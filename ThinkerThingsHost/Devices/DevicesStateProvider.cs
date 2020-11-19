using System;
using System.Collections.Generic;
using System.Linq;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Common;
using ThinkerThings.DataContracts.Devices;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.Devices
{
    public class DevicesStateProvider : IDevicesStateProvider
    {
        private readonly List<GenericConfiguration> _devices = new List<GenericConfiguration>();
        private readonly object _lock = new object();

        public GenericConfiguration GetDeviceById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            lock (_lock)
            {
                return FindDeviceById(id);
            }
        }

        public GenericConfiguration GetDeviceByName(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName));

            lock (_lock)
            {
                return FindDeviceByName(deviceName);
            }
        }

        public IDevicePortState GetStateOfPort(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentNullException(nameof(portName));

            lock (_lock)
            {
                var port = GetPort(portName);
                if (port == null)
                    return null;

                return new DevicePortState(port.Item1, port.Item2);
            }
        }

        public void LoadDevice(GenericConfiguration genericConfiguration)
        {
            if (genericConfiguration == null)
                throw new ArgumentNullException(nameof(genericConfiguration));

            lock (_lock)
            {
                InternalLoadDevice(genericConfiguration);
            }
        }

        public void UpdatePortState(ChangePortStateContext changePortStateContext, string requesterId)
        {
            if (changePortStateContext == null)
                throw new ArgumentNullException(nameof(changePortStateContext));
            if (string.IsNullOrWhiteSpace(requesterId))
                throw new ArgumentNullException(requesterId);

            lock (_lock)
            {
                InternalUpdatePortState(changePortStateContext, requesterId);
            }
        }

        private GenericConfiguration FindDeviceByName(string deviceName)
        {
            return _devices.FirstOrDefault(p => string.Equals(p.DeviceName, deviceName, StringComparison.InvariantCultureIgnoreCase));
        }

        private GenericConfiguration FindDeviceById(string id)
        {
            return _devices.FirstOrDefault(p => string.Equals(p.ConnectionId, id));
        }

        private void InternalLoadDevice(GenericConfiguration genericConfiguration)
        {
            _devices.RemoveAll(p => string.Equals(p.DeviceName, genericConfiguration.DeviceName, StringComparison.InvariantCultureIgnoreCase));

            _devices.Add(genericConfiguration);
        }

        private void InternalUpdatePortState(ChangePortStateContext changePortStateContext, string requesterId)
        {
            var device = FindDeviceById(requesterId);

            if (device == null)
                throw new InvalidOperationException($"Não foi encontrado o dispositico com o Id '{requesterId}'");

            var port = device.Ports.FirstOrDefault(p => string.Equals(p.PortName, changePortStateContext.TargetPortName, StringComparison.InvariantCultureIgnoreCase));
            if (port == null)
                throw new InvalidOperationException($"Não foi encontrado a porta '{changePortStateContext.TargetPortName}' no dispositivo '{device.DeviceName}'");

            port.PortState = changePortStateContext.SetState;
        }

        private IEnumerable<Tuple<DevicePort, GenericConfiguration>> GetAllPorts()
        {
            foreach (var device in _devices)
            {
                foreach (var port in device.Ports)
                {
                    yield return Tuple.Create(port, device);
                }
            }
        }

        private Tuple<DevicePort, GenericConfiguration> GetPort(string portName)
        {
            return GetAllPorts().FirstOrDefault(p => string.Equals(p.Item1.PortName, portName, StringComparison.InvariantCultureIgnoreCase));
        }

        public GenericConfiguration[] GetAllDevices()
        {
            return _devices.ToArray();
        }

        private class DevicePortState : IDevicePortState
        {
            private readonly DevicePort _port;
            private readonly GenericConfiguration _genericConfiguration;

            public DevicePortState(DevicePort port, GenericConfiguration genericConfiguration)
            {
                _port = port;
                _genericConfiguration = genericConfiguration;
            }

            public PortStates PortState => _port.PortState;
            public string DeviceName => _genericConfiguration.DeviceName;
            public string ConnectionId => _genericConfiguration.ConnectionId;
            public PortTypes PortType => _port.PortType;
            public PortStates DefaultPortState => _port.DefaultPortState;
        }

    }

}
