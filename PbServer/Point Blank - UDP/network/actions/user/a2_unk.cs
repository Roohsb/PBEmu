using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a2_unk
    {
        public class Struct
        {
            public ushort _unkV;
        }
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _unkV = p.readUH()
            }; 
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            s.WriteH(ReadInfo(ac, p, genLog)._unkV);
        }
    }
}