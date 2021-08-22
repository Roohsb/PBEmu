using Battle.data.models;
using Battle.data.sync;

namespace Battle.network.actions.user
{
    public class a100000_PassPortal
    {
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _portal = p.readUH()
            };
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(2);
        }
        public static void SendPassSync(Room room, Player p, Struct info)
        {
            Battle_SyncNet.SendPortalPass(room, p, info._portal);
        }
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            writeInfo(s, ReadInfo(ac, p, genLog));
        }
        public static void writeInfo(SendPacket s, Struct info)
        {
            s.WriteH(info._portal);
        }
        public class Struct
        {
            public ushort _portal;
        }
    }
}