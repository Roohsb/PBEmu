namespace Battle.network.actions.others
{
    public class Code9_StageInfoObjStatic
    {
        public class Struct
        {
            public byte _isDestroyed;
        }
        public static Struct ReadSyncInfo(ReceivePacket p, bool genLog) =>
             new Struct
             {
                 _isDestroyed = p.readC() //0=Normal|1=Quebrado
             };
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            s.WriteC(ReadSyncInfo(p, genLog)._isDestroyed);
        }
    }
}