using ThinkerThings.DataContracts.Commands;

namespace ThinkerThings.SignalRInterfaces
{
    public interface IThinkerThingsSignalRHost
    {
        void RequireConfiguration(string deviceName);
        void UpdatePortState(ChangePortStateContext changePortStateContext);
    }

}
