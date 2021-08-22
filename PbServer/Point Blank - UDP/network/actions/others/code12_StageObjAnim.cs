namespace Battle.network.actions.others
{
    public class Code12_StageObjAnim
    {
        public class Struct
        {
            public byte _unk, _anim1, _anim2;
            public float _syncDate;
            public ushort _life;
        }
        public static byte[] ReadInfo(ReceivePacket p) => p.readB(9);
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
                       new Struct
                       {
                           _unk = p.readC(),
                           _life = p.readUH(),
                           _syncDate = p.readT(),
                           _anim1 = p.readC(),
                           _anim2 = p.readC()
                       };
        public static void WriteInfo(SendPacket s, ReceivePacket p)
        {
            s.WriteB(ReadInfo(p));
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteC(info._unk);
            s.WriteH(info._life);
            s.WriteT(info._syncDate);
            s.WriteC(info._anim1);
            s.WriteC(info._anim2);
        }
    }
}