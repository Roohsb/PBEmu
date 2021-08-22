namespace Battle.network.actions.user
{
    public class a100_LifeSync
    {
        public static ushort ReadInfo(ReceivePacket p, bool genLog) => p.readUH();
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(2);
        }
        public static void writeInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            s.WriteH(ReadInfo(p, genLog));
        }
    }
}