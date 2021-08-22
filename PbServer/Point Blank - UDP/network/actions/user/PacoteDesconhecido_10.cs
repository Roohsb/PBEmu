using Battle.data.models;

namespace Battle.network.actions.user
{
    public class PacoteDesconhecido_10
    {
        public class Struct
        {
            public byte _unkV, _unkV2;
        }
        public static Struct ReadSyncInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _unkV = p.readC(),
                _unkV2 = p.readC()
            };
        public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            Struct info = ReadSyncInfo(ac, p, genLog);
            s.WriteC(info._unkV);
            s.WriteC(info._unkV2);
        }
    }
}
