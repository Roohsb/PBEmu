namespace Battle.network.actions.others
{
    public class Code6_ObjectAnim
    {
        public class Struct
        {
            public float _syncDate;
            public byte _anim1, _anim2;
            public ushort _life;
        }
        public static byte[] ReadInfo(ReceivePacket p) => p.readB(8);
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
            new Struct
            {
                _life = p.readUH(),
                _anim1 = p.readC(),
                _anim2 = p.readC(),
                _syncDate = p.readT()
            };
        public static void WriteInfo(SendPacket s, ReceivePacket p)
        {
            s.WriteB(ReadInfo(p));
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteH(info._life);
            s.WriteC(info._anim1);
            s.WriteC(info._anim2);
            s.WriteT(info._syncDate);
        }
    }
}