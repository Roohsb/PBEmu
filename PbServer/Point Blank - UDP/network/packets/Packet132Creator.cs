using Battle.data;
using Battle.data.enums;
using Battle.data.models;
using Battle.network.actions.others;
using System;
using System.IO;

namespace Battle.network.packets
{
    public class Packet132Creator
    {
        public static byte[] GetBaseData132(byte[] data)
        {
            int count = 0;
            ReceivePacket p = new ReceivePacket(data);
            using (SendPacket s = new SendPacket())
            {
                s.WriteT(p.readT());
                do
                {
                    ActionModel ac = new ActionModel();
                    try
                    {
                        ac._type = (P2P_SUB_HEAD)p.readC(out bool exception);
                        if (exception) break;
                        ac._slot = p.readUH();
                        ac._lengthData = p.readUH();
                        if (ac._lengthData == 65535)
                            break;
                        s.WriteC((byte)ac._type);
                        s.WriteH(ac._slot);
                        s.WriteH(ac._lengthData);
                        switch (ac._type)
                        {
                            case P2P_SUB_HEAD.GRENADE:Code1_GrenadeSync.WriteInfo(s, p);break;
                            case P2P_SUB_HEAD.DROPEDWEAPON:Code2_WeaponSync.WriteInfo(s, p);break;
                            case P2P_SUB_HEAD.OBJECT_STATIC:code3_ObjectStatic.writeInfo(s, p);break;
                            case P2P_SUB_HEAD.OBJECT_ANIM:Code6_ObjectAnim.WriteInfo(s, p);break;
                            case P2P_SUB_HEAD.STAGEINFO_OBJ_STATIC:Code9_StageInfoObjStatic.WriteInfo(s, p, false); break;
                            case P2P_SUB_HEAD.STAGEINFO_OBJ_ANIM:Code12_StageObjAnim.WriteInfo(s, p);break;
                            case P2P_SUB_HEAD.CONTROLED_OBJECT:code13_ControledObj.WriteInfo(s, p, false);break;
                            case P2P_SUB_HEAD p2P_SUB_HEAD when (p2P_SUB_HEAD == P2P_SUB_HEAD.USER || p2P_SUB_HEAD == P2P_SUB_HEAD.STAGEINFO_CHARA):
                                {
                                    ac._flags = (Events)p.readUD();
                                    ac._data = p.readB(ac._lengthData - 9);
                                    s.WriteD((uint)ac._flags);
                                    s.WriteB(ac._data);
                                    if (ac._data.Length == 0 && (uint)ac._flags != 0)
                                        break;
                                    break;
                                }
                            default:
                                {
                                    Logger.Warning("[New user packet type2 '" + ac._type + "' or '" + (int)ac._type + "']: " + BitConverter.ToString(data));
                                    throw new Exception("Unknown action type2");
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning("B: " + BitConverter.ToString(data));
                        Logger.Warning(ex.ToString());
                        s.mstream = new MemoryStream();
                        break;
                    }
                }
                while (++count < 16);
                return s.mstream.ToArray();
            }
        }
        public static byte[] GetCode132(byte[] data, DateTime time, int round, int slot)
        {
            using (SendPacket s = new SendPacket())
            {
                byte[] actions = GetBaseData132(data);
                s.WriteC(132); //opcode
                s.WriteC((byte)slot); //slot
                s.WriteT(AllUtils.GetDuration(time));
                s.WriteC((byte)round);
                s.WriteH((ushort)(13 + actions.Length)); //lengthActionData
                s.WriteD(0);
                s.WriteB(AllUtils.Encrypt(actions, (13 + actions.Length) % 6 + 1));
                return s.mstream.ToArray();
            }
        }
    }
}