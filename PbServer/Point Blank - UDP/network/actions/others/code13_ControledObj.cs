namespace Battle.network.actions.others
{
    public class code13_ControledObj
    {
        public class Struct
        {
            public byte[] _unk;
        }
        public static Struct ReadSyncInfo(ReceivePacket p, bool genLog)
            => new Struct
            {
                _unk = p.readB(9)
            };
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            s.WriteB(ReadSyncInfo(p, genLog)._unk);
        }
    }
}