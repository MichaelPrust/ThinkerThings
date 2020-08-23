using Windows.Devices.Gpio;

namespace ThinkerThingsRaspy.PinManager.Pins
{
    internal class OutputSwitchPin : OutputPin
    {
        public OutputSwitchPin(GpioPin gpioPin) : base(gpioPin)
        {
        }

        protected override void ChangeValue(GpioPinValue value)
        {
            _gpioPin.Write(value);
        }

    }

}