using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a40000_DeathData
    {
        public class Struct
        {
            public byte _deathType, _hitPart;
            public ushort _camX, _camY, _camZ;
            public int _weaponId;
        }
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            {
                _deathType = p.readC(),
                _hitPart = p.readC(),
                _camX = p.readUH(),
                _camY = p.readUH(),
                _camZ = p.readUH(),
                _weaponId = p.readD()
            };
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(12);
        }
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            var info = ReadInfo(ac, p, genLog);
            s.WriteC(info._deathType);
            s.WriteC(info._hitPart);
            s.WriteH(info._camX);
            s.WriteH(info._camY);
            s.WriteH(info._camZ);
            s.WriteD(info._weaponId);
        }
    }
}