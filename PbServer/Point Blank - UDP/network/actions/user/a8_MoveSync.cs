using Battle.data.enums;
using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a8_MoveSync
    {
        public class Struct
        {
            public CharaMoves _spaceFlags;
            public ushort _objId;
        }
        public static Struct ReadSyncInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _spaceFlags = (CharaMoves)p.readC(),
                _objId = p.readUH()
            };
        public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            WriteInfo(s, ReadSyncInfo(ac, p, genLog));
        }
        public static void WriteInfo(SendPacket s, Struct info)
        {
            s.WriteC((byte)info._spaceFlags);
            s.WriteH(info._objId);
        }
    }
}