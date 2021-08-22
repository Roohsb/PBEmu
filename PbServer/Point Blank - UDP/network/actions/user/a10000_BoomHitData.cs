using Battle.data;
using Battle.data.enums;
using Battle.data.enums.weapon;
using SharpDX;
using System.Collections.Generic;

namespace Battle.network.actions.user
{
    public class a10000_BoomHitData
    {
        /// <summary>
        /// Puxa todas as informações. OnlyBytes desativado.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="genLog"></param>
        /// <param name="OnlyBytes">Não calcular informações adicionais?</param>
        /// <returns></returns>
        public static List<HitData> ReadInfo(ReceivePacket p, bool genLog, bool OnlyBytes = false) => BaseReadInfo(p, OnlyBytes, genLog);
        public static void ReadInfo(ReceivePacket p)
        {;
            p.Advance(24 * p.readC());
        }
        private static List<HitData> BaseReadInfo(ReceivePacket p, bool OnlyBytes, bool genLog)
        {
            List<HitData> hits = new List<HitData>();
            int objsCount = p.readC();
            for (int i = 0; i < objsCount; i++)
            {
                HitData hit = new HitData
                {
                    _hitInfo = p.readUD(),
                    _boomInfo = p.readUH(),
                    _weaponInfo = p.readUH(),
                    _weaponSlot = p.readC(),
                    _deathType = p.readC(),
                    FirePos = p.readUHVector(),
                    HitPos = p.readUHVector(),
                    _grenadesCount = p.readUH()
                };
                if (!OnlyBytes)
                {
                    hit.HitEnum = (HitType)AllUtils.GetHitHelmet(hit._hitInfo);
                    if (hit._boomInfo > 0)
                    {
                        hit.BoomPlayers = new List<int>();
                        for (int s = 0; s < 16; s++)
                        {
                            int flag = (1 << s);
                            if ((hit._boomInfo & flag) == flag)
                                hit.BoomPlayers.Add(s);
                        }
                    }
                    hit.WeaponClass = (ClassType)(hit._weaponInfo & 63);
                    hit.WeaponId = (hit._weaponInfo >> 6);
                }
                if (genLog)
                {
                    Logger.Warning("[Player pos] X: " + hit.FirePos.X + "; Y: " + hit.FirePos.Y + "; Z: " + hit.FirePos.Z);
                    Logger.Warning("[Object pos] X: " + hit.HitPos.X + "; Y: " + hit.HitPos.Y + "; Z: " + hit.HitPos.Z);
                    //Logger.warning("[" + i + "] Slot " + aM._slot + " explosive BOOM: " + hit._objInfo + ";" + hit._hitInfo + ";" + hit._weaponDamage + ";" + hit._weaponDamageC + ";" + hit._weaponInfo + ";" + hit._weaponSlot + ";" + hit._deathType + ";" + hit._playerPos._x + ";" + hit._playerPos._y + ";" + hit._playerPos._z + ";" + hit._objPos._x + ";" + hit._objPos._y + ";" + hit._objPos._z + ";" + hit._u6);
                }
                hits.Add(hit);
            }
            return hits;
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
                s.WriteH(hit._boomInfo);
                s.WriteH(hit._weaponInfo);
                s.WriteC(hit._weaponSlot);
                s.WriteC(hit._deathType);
                s.WriteHVector(hit.FirePos);
                s.WriteHVector(hit.HitPos);
                s.WriteH(hit._grenadesCount);
            }
        }
        public class HitData
        {
            public byte _weaponSlot, _deathType;
            public ushort _boomInfo, _grenadesCount, _weaponInfo;
            public uint _hitInfo;
            public int WeaponId;
            public List<int> BoomPlayers;
            public Half3 FirePos, HitPos;
            public HitType HitEnum;
            public ClassType WeaponClass;
        }
    }
}