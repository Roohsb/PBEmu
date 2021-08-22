using Battle.data.enums.weapon;
using SharpDX;
using System.Collections.Generic;

namespace Battle.network.actions.user
{
    public class a200_SuicideDamage
    {
        public class HitData
        {
            public uint _hitInfo;
            public ushort _weaponInfo;
            public Half3 PlayerPos;
            public byte _weaponSlot;
            public ClassType WeaponClass;
            public int WeaponId;
        }
        public static List<HitData> ReadInfo(ReceivePacket p, bool genLog, bool OnlyBytes = false) => BaseReadInfo(p, OnlyBytes, genLog);
        private static List<HitData> BaseReadInfo(ReceivePacket p, bool OnlyBytes, bool genLog)
        {
            List<HitData> hits = new List<HitData>();
            int count = p.readC();
            for (int i = 0; i < count; i++)
            {
                HitData hit = new HitData
                {
                    _hitInfo = p.readUD(),
                    _weaponInfo = p.readUH(),
                    _weaponSlot = p.readC(),
                    PlayerPos = p.readUHVector()
                };
                if (!OnlyBytes)
                {
                    hit.WeaponClass = (ClassType)((hit._weaponInfo >> 32) & 63); //Funcional? Antigo = >> 32) & 31 | Novo = >> 32) & 63
                    hit.WeaponId = (hit._weaponInfo >> 6);
                }
                if (genLog)
                {
                    Logger.Warning("[" + i + "] Committed suicide: hitinfo,weaponinfo,weaponslot,camX,camY,camZ (" + hit._hitInfo + ";" + hit._weaponInfo + ";" + hit._weaponSlot + ";" + hit.PlayerPos.X + ";" + hit.PlayerPos.Y + ";" + hit.PlayerPos.Z + ")");
                }
                hits.Add(hit);
            }
            return hits;
        }
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(13 * p.readC());
        }
        public static void writeInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            writeInfo(s, ReadInfo(p, genLog, true));
        }
        public static void writeInfo(SendPacket s, List<HitData> hits)
        {
            s.WriteC((byte)hits.Count);
            for (int i = 0; i < hits.Count; i++)
            {
                HitData hit = hits[i];
                s.WriteD(hit._hitInfo);
                s.WriteH(hit._weaponInfo);
                s.WriteC(hit._weaponSlot);
                s.WriteHVector(hit.PlayerPos);
            }
        }
    }
}