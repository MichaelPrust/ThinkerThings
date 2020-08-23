using Windows.Devices.Gpio;

namespace ThinkerThingsRaspy.PinManager.Pins
{
    internal abstract class Pin
    {
        protected readonly GpioPin _gpioPin;
        protected bool _disposed = false;

        protected Pin(GpioPin gpioPin)
        {
            _gpioPin = gpioPin;
        }

        public int PinId => _gpioPin.PinNumber;

        public virtual void ReleasePin()
        {
            _disposed = true;
            _gpioPin.Dispose();
        }

    }

}
