using ThinkerThings.DataContracts.Common;

namespace ThinkerThingsRaspy.PinManager.Pins
{
    internal class PinConfiguration
    {
        private PinConfiguration(RaspyberryPiPort raspyberryPiPort)
        {
            PortName = raspyberryPiPort.PortName;
            PortType = raspyberryPiPort.PortType;
            PinId = raspyberryPiPort.PinId;
        }

        public static PinConfiguration Create(RaspyberryPiPort raspyberryPiPort)
        {
            return new PinConfiguration(raspyberryPiPort);
        }

        public string PortName { get;}
        public PortTypes PortType { get;}
        public int PinId { get;}
    }

}
