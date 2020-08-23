using Windows.Devices.Gpio;

namespace ThinkerThingsRaspy.PinManager.Pins
{
    internal abstract class OutputPin : Pin
    {
        protected OutputPin(GpioPin gpioPin) : base(gpioPin)
        {
        }

        public GpioPinValue GpioPinValue 
        { 
            get
            {
                return _gpioPin.Read();
            }
            set
            {
                if (value != _gpioPin.Read())
                {
                    ChangeValue(value);
                }
            }
        }

        public static OutputPin CreateSwitch(int pinId)
        {
            var gpioController = GpioController.GetDefault();
            var gpioPin = gpioController.OpenPin(pinId);
            gpioPin.SetDriveMode(GpioPinDriveMode.Output);

            var result = new OutputSwitchPin(gpioPin);
            return result;
        }

        public static OutputPin CreatePulse(int pinId, GpioPinValue defaultGpioPinValue, int pulseLength)
        {
            var gpioController = GpioController.GetDefault();
            var gpioPin = gpioController.OpenPin(pinId);
            gpioPin.SetDriveMode(GpioPinDriveMode.Output);

            var result = new OutputPulsePin(gpioPin, defaultGpioPinValue, pulseLength);
            return result;
        }

        protected abstract void ChangeValue(GpioPinValue value);
    }

}