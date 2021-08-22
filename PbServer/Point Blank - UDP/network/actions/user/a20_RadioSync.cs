using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a20_RadioSync
    {
        public static Struct readSyncInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _radioId = p.readC(),
                _areaId = p.readC()
            };
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            Struct info = readSyncInfo(ac, p, genLog);
            s.WriteC(info._radioId);
            s.WriteC(info._areaId);
        }
        public class Struct
        {
            public byte _radioId, _areaId;
        }
    }
}