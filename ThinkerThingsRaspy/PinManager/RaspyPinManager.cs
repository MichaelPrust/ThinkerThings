using System;
using System.Collections.Generic;
using System.Linq;
using ThinkerThings.DataContracts.Commands;
using ThinkerThings.DataContracts.Common;
using ThinkerThingsRaspy.PinManager.Pins;
using ThinkerThingsRaspy.SignalRClient;
using Windows.Devices.Gpio;

namespace ThinkerThingsRaspy.PinManager
{
    internal class RaspyPinManager
    {
        private readonly List<OutputPin> _outputPins = new List<OutputPin>();

        private readonly List<PinConfiguration> _pinConfigurations = new List<PinConfiguration>();

        private RaspyPinManager()
        {
        }

        public static RaspyPinManager Instance { get; } = new RaspyPinManager();

        public void LoadConfiguration(RaspyberryPiPort[] raspyberryPiPorts)
        {
            if (!_pinConfigurations.Any())
            {
                var newConfiguration = raspyberryPiPorts.Select(PinConfiguration.Create);
                CreateAppendAndConfigure(newConfiguration, raspyberryPiPorts);
            }
            else
            {
                var currentConfiguration = _pinConfigurations;
                var newConfiguration = raspyberryPiPorts.Select(PinConfiguration.Create).ToArray();

                var comparer = new PinConfigurationComparer();

                var removed = currentConfiguration.Except(newConfiguration, comparer);
                var newEntries = newConfiguration.Except(currentConfiguration, comparer);

                Remove(removed);
                CreateAppendAndConfigure(newEntries, raspyberryPiPorts);
            }
        }

        public DevicePort[] GetDevicePorts()
        {
            var result = new List<DevicePort>();
            foreach (var pinConfiguration in _pinConfigurations)
            {

                var item = new DevicePort()
                {
                    PortName = pinConfiguration.PortName,
                    PortType = pinConfiguration.PortType
                };
                switch (pinConfiguration.PortType)
                {
                    case PortTypes.Switch:
                    case PortTypes.Pulse:
                        var outputPin = _outputPins.First(p => p.PinId == pinConfiguration.PinId);
                        item.PortState = GpioPinValueToPortState(outputPin.GpioPinValue);
                        break;
                }
                result.Add(item);
            }
            return result.ToArray();
        }

        internal void UpdatePortState(Pin pin, GpioPinValue gpioPinValue)
        {
            var portState = GpioPinValueToPortState(gpioPinValue);
            var portName = _pinConfigurations.FirstOrDefault(p => p.PinId == pin.PinId).PortName;

            var context = new ChangePortStateContext()
            {
                SetState = portState,
                TargetPortName = portName
            };

            SignalRRaspyberryPiClient.Instance.UpdatePortState(context);
        }

        internal void ChangePortState(ChangePortStateContext context)
        {
            ChangePortState(context.TargetPortName, context.SetState);
            SignalRRaspyberryPiClient.Instance.UpdatePortState(context);
        }

        private void ChangePortState(string portName, PortStates portState)
        {
            var pinConfiguration = _pinConfigurations.FirstOrDefault(p => p.PortName == portName);
            ChangePortState(pinConfiguration, portState);
        }

        public void ChangePortState(int pinId, PortStates portState)
        {
            var pinConfiguration = _pinConfigurations.FirstOrDefault(p => p.PinId == pinId);
            ChangePortState(pinConfiguration, portState);
        }

        private void ChangePortState(PinConfiguration pinConfiguration, PortStates portState)
        {
            switch (pinConfiguration.PortType)
            {
                case PortTypes.Switch:
                case PortTypes.Pulse:
                    var outputPin = _outputPins.First(p => p.PinId == pinConfiguration.PinId);
                    ChangePortState(outputPin, portState);
                    break;
            }
        }

        private void ChangePortState(OutputPin outputPin, PortStates portState)
        {
            var gpioPinValue = PortStateToGpioPinValue(portState);
            if (gpioPinValue.HasValue)
            {
                outputPin.GpioPinValue = gpioPinValue.Value;
            }
        }

        private PortStates GpioPinValueToPortState(GpioPinValue gpioPinValue)
        {
            PortStates result;
            switch (gpioPinValue)
            {
                case GpioPinValue.Low:
                    result = PortStates.Actived;
                    break;
                case GpioPinValue.High:
                    result = PortStates.Deactived;
                    break;
                default:
                    result = PortStates.None;
                    break;
            }

            return result;
        }

        private GpioPinValue? PortStateToGpioPinValue(PortStates portState)
        {
            GpioPinValue? result;
            switch (portState)
            {
                case PortStates.Actived:
                    result = GpioPinValue.Low;
                    break;
                case PortStates.Deactived:
                    result = GpioPinValue.High;
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private void Remove(IEnumerable<PinConfiguration> pinConfigurations)
        {
            foreach (var pinConfiguration in pinConfigurations)
            {
                var outputPin = _outputPins.FirstOrDefault(p => p.PinId == pinConfiguration.PinId);
                if (outputPin != null)
                {
                    outputPin.ReleasePin();
                    _outputPins.Remove(outputPin);
                }

                _pinConfigurations.RemoveAll(p => p.PinId == pinConfiguration.PinId);
            }
        }

        private void CreateAppendAndConfigure(IEnumerable<PinConfiguration> pinConfigurations, RaspyberryPiPort[] raspyberryPiPorts)
        {
            foreach (var pinConfiguration in pinConfigurations)
            {
                var port = raspyberryPiPorts.First(p => p.PinId == pinConfiguration.PinId);
                switch (pinConfiguration.PortType)
                {
                    case PortTypes.Switch:
                        _outputPins.Add(OutputPin.CreateSwitch(pinConfiguration.PinId));
                        break;
                    case PortTypes.Pulse:
                        var defaultGpioValue = PortStateToGpioPinValue(port.DefaultPortState);
                        _outputPins.Add(OutputPin.CreatePulse(pinConfiguration.PinId, defaultGpioValue.Value, port.PulseLength));
                        break;
                }

                _pinConfigurations.Add(pinConfiguration);
                ChangePortState(port.PortName, port.PortState);
            }
        }

        private class PinConfigurationComparer : IEqualityComparer<PinConfiguration>
        {
            public bool Equals(PinConfiguration x, PinConfiguration y)
            {
                var nameA = x.PortName;
                var nameB = y.PortName;
                var portTypeA = x.PortType;
                var portTypeB = y.PortType;
                var pinIdA = x.PinId;
                var pinIdB = y.PinId;

                return string.Equals(nameA, nameB) && portTypeA == portTypeB && pinIdA == pinIdB;
            }

            public int GetHashCode(PinConfiguration obj)
            {
                var stringId = $"{obj.PortName}-{obj.PortType}-{obj.PinId}";
                return stringId.GetHashCode();
            }
        }

    }

}