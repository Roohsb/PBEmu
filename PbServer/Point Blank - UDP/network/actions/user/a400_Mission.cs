using Battle.data.enums.bomb;
using Battle.data.models;
using Battle.data.sync;

namespace Battle.network.actions.user
{
    public class a400_Mission
    {
        public class Struct
        {
            public int _bombAll, BombId;
            public BombFlag BombEnum;
            public float _plantTime;
        }
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog, float time, bool OnlyBytes = false)
        {
            Struct info = new Struct
            {
                _bombAll = p.readC(),
                _plantTime = p.readT()
            };
            if (!OnlyBytes)
            {
                info.BombEnum = (BombFlag)(info._bombAll & 15);
                info.BombId = (info._bombAll >> 4);
            }
            if (genLog)
            {
                Logger.Warning("Slot " + ac._slot + " bomb: (" + info.BombEnum + "; Id: " + info.BombId + "; sTime: " + info._plantTime + "; aTime: " + time + ")");
            }
            return info;
        }
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(5);
        }
        public static void SendC4UseSync(Room room, Player pl, Struct info)
        {
            if (pl == null)
                return;
            Battle_SyncNet.SendBombSync(room, pl, info.BombEnum.HasFlag(BombFlag.Defuse) ? 1 : 0, info.BombId);
        }
        public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog, float pacDate, float plantDuration)
        {
            Struct info = ReadInfo(ac, p, genLog, pacDate);
            if (info._plantTime > 0 && pacDate >= info._plantTime + (plantDuration) && !info.BombEnum.HasFlag(BombFlag.Stop))
                info._bombAll += 2;
            WriteInfo(s, info);
        }
        public static void WriteInfo(SendPacket s, Struct info)
        {
            s.WriteC((byte)info._bombAll);
            s.WriteT(info._plantTime);
        }
    }
}