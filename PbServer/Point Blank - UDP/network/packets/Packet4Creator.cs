using Battle.data;
using Battle.data.enums;
using Battle.data.models;
using System;
using System.Collections.Generic;

namespace Battle.network.packets
{
    public class Packet4Creator
    {
        public static byte[] GetCode4(byte[] actions, DateTime date, int round, int slot) => 
            BaseGetCode4(AllUtils.Encrypt(actions, (13 + actions.Length) % 6 + 1), date, round, slot);
        private static byte[] BaseGetCode4(byte[] actionsBuffer, DateTime date, int round, int slot)
        {
            using (SendPacket s = new SendPacket())
            {
                s.WriteC(4);
                s.WriteC((byte)slot);
                s.WriteT(AllUtils.GetDuration(date));
                s.WriteC((byte)round);
                s.WriteH((ushort)(13 + actionsBuffer.Length));
                s.WriteD(0);
                s.WriteB(actionsBuffer);
                return s.mstream.ToArray();
            }
        }
        public static byte[] GetCode4SyncData(List<ObjectHitInfo> objs)
        {
            int objscount = 0;
            using (SendPacket s = new SendPacket())
            {
                do
                {
                    ObjectHitInfo obj = objs[objscount];
                    switch (obj.syncType)
                    {
                        case 1:
                            {
                                if (obj.objSyncId == 0)
                                {
                                    s.WriteC((byte)P2P_SUB_HEAD.OBJECT_STATIC);
                                    s.WriteH((ushort)obj.objId);
                                    s.WriteH(8);
                                    s.WriteH((ushort)obj.objLife);
                                    s.WriteC((byte)obj.killerId);
                                }
                                else
                                {
                                    s.WriteC((byte)P2P_SUB_HEAD.OBJECT_ANIM);
                                    s.WriteH((ushort)obj.objId);
                                    s.WriteH(13);
                                    s.WriteH((ushort)obj.objLife);
                                    s.WriteC((byte)obj._animId1);
                                    s.WriteC((byte)obj._animId2);
                                    s.WriteT(obj._specialUse);
                                }
                                break;
                            }
                        case 2:
                            {
                                Events events = Events.LifeSync;
                                int tamanho = 11;
                                if (obj.objLife == 0)
                                {
                                    events |= Events.Death;
                                    tamanho += 12;
                                }
                                s.WriteC((byte)P2P_SUB_HEAD.USER);
                                s.WriteH((ushort)obj.objId);
                                s.WriteH((ushort)tamanho);
                                s.WriteD((uint)events);
                                s.WriteH((ushort)obj.objLife);
                                if (events.HasFlag(Events.Death))
                                {
                                    s.WriteC((byte)(obj.deathType + (obj.objId * 16)));
                                    s.WriteC((byte)obj.hitPart);
                                    s.WriteH(0);
                                    s.WriteH(0);
                                    s.WriteH(0);
                                    s.WriteD(obj.weaponId);
                                }
                                break;
                            }
                        case 3:
                            {
                                if (obj.objSyncId == 0)
                                {
                                    s.WriteC((byte)P2P_SUB_HEAD.STAGEINFO_OBJ_STATIC);
                                    s.WriteH((ushort)obj.objId);
                                    s.WriteH(6);
                                    s.WriteC((obj.objLife == 0));
                                }
                                else
                                {
                                    s.WriteC((byte)P2P_SUB_HEAD.STAGEINFO_OBJ_ANIM);
                                    s.WriteH((ushort)obj.objId);
                                    s.WriteH(14);
                                    s.WriteC((byte)obj._destroyState);
                                    s.WriteH((ushort)obj.objLife);
                                    s.WriteT(obj._specialUse);
                                    s.WriteC((byte)obj._animId1);
                                    s.WriteC((byte)obj._animId2);
                                }
                                break;
                            }
                        case 4:
                            {
                                s.WriteC((byte)P2P_SUB_HEAD.STAGEINFO_CHARA);
                                s.WriteH((ushort)obj.objId);
                                s.WriteH(11);
                                s.WriteD(256);
                                s.WriteH((ushort)obj.objLife);
                                break;
                            }
                        case 5:
                            {
                                s.WriteC((byte)P2P_SUB_HEAD.USER);
                                s.WriteH((short)obj.objId);
                                s.WriteH(11);
                                s.WriteD(524288);
                                s.WriteC((byte)(obj.killerId + ((int)obj.deathType * 16)));
                                s.WriteC((byte)obj.objLife);
                                break;
                            }
                    }
                }
                while (++objscount < objs.Count);
                return s.mstream.ToArray();
            }
        }
    }
}