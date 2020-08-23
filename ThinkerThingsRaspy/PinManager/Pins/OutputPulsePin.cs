using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace ThinkerThingsRaspy.PinManager.Pins
{
    internal class OutputPulsePin : OutputPin
    {
        private readonly GpioPinValue _defaultGpioPinValue;
        private readonly int _pulseLength;
        private int _pulseId = 0;

        public OutputPulsePin(GpioPin gpioPin, GpioPinValue defaultGpioPinValue, int delay) : base(gpioPin)
        {
            _defaultGpioPinValue = defaultGpioPinValue;
            _pulseLength = delay;
        }

        protected override void ChangeValue(GpioPinValue value)
        {
            _gpioPin.Write(GpioPinValue);
            _pulseId++;
            var pulseId = _pulseId;
            Task.Run(() => PulseControl(pulseId));
        }

        private async Task PulseControl(int pulseId)
        {
            await Task.Delay(_pulseLength);
            if (!_disposed && (_pulseId == pulseId))
                _gpioPin.Write(_defaultGpioPinValue);

        }

    }

}