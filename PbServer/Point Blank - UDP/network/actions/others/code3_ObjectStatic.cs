namespace Battle.network.actions.others
{
    public class code3_ObjectStatic
    {
        public static byte[] ReadInfo(ReceivePacket p) => p.readB(3);
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
            new Struct
            {
                Life = p.readUH(),
                DestroyedBySlot = p.readC()
            };
        public static void writeInfo(SendPacket s, ReceivePacket p)
        {
            s.WriteB(ReadInfo(p));
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteH(info.Life);
            s.WriteC(info.DestroyedBySlot);
        }
        public class Struct
        {
            public byte DestroyedBySlot;
            public ushort Life;
        }
    }
}