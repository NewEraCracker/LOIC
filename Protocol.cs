namespace LOIC
{
    public enum Protocol
    {
        Unknown = 0,
        Tcp,
        Udp,
        Http
    }

    public static class ProtocolHelper
    {
        public static Protocol Read(string method)
        {
            switch (method.ToUpperInvariant())
            {
                case "TCP":
                    return Protocol.Tcp;

                case "UDP":
                    return Protocol.Udp;

                case "HTTP":
                    return Protocol.Http;

                default:
                    return Protocol.Unknown;
            }
        }
    
    }
}
