
namespace ThinkerThingsHost.AlexaServices
{
    public static class AlexaIntents
    {
        public static class ActivateIntent
        {
            public const string Name = "activateIntent";
            public static class Fields
            {
                public const string PortNameSlotName = "portName";
            }
        }

        public static class DeactivateIntent
        {
            public const string Name = "deactivateIntent";
            public static class Fields
            {
                public const string PortNameSlotName = "portName";
            }
        }

        public static class PulseIntent
        {
            public const string Name = "pulseIntent";
            public static class Fields
            {
                public const string PortNameSlotName = "portName";
            }
        }

        public static class GetStatusIntent
        {
            public const string Name = "getStatusIntent";
            public static class Fields
            {
                public const string PortNameSlotName = "portName";
            }
        }

        public static class YesIntent
        {
            public const string Name = "simIntent";
            public static class Fields
            {
            }
        }

    }

}
