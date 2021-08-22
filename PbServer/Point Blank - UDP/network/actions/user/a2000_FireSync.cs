using System;

namespace Battle.network.actions.user
{
    public class a2000_FireSync
    {
        public class Struct
        {
            public ushort _shotId, _shotIndex, _camX, _camY, _camZ;
            public int _weaponNumber;
        }
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
             new Struct
             {
                 _shotId = p.readUH(),
                 _shotIndex = p.readUH(),
                 _camX = p.readUH(),
                 _camY = p.readUH(),
                 _camZ = p.readUH(),
                 _weaponNumber = p.readD(), //weaponId
             };
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(14);
        }
        public static void writeInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteH(info._shotId);
            s.WriteH(info._shotIndex);
            s.WriteH(info._camX);
            s.WriteH(info._camY);
            s.WriteH(info._camZ);
            s.WriteD(info._weaponNumber);
        }
    }
}