namespace Battle.network.actions.user
{
    public class PacoteDesconhecido_1
    {
        public class Struct
        {
            public byte _unkV, _unkV2, _unkV3, _unkV4;
        }
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>new Struct
             { _unkV = p.readC(), _unkV2 = p.readC(),_unkV3 = p.readC(), _unkV4 = p.readC()};
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            WriteInfo(s, ReadInfo(p, genLog));
        }
        public static void WriteInfo(SendPacket s, Struct info)
        {
            s.WriteC(info._unkV);
            s.WriteC(info._unkV2);
            s.WriteC(info._unkV3);
            s.WriteC(info._unkV4);
        }
    }
}