using Battle.config;
using Battle.data;
using Battle.data.enums;
using Battle.data.enums.bomb;
using Battle.data.enums.weapon;
using Battle.data.models;
using Battle.data.sync;
using Battle.data.sync.client_side;
using Battle.data.xml;
using Battle.network.actions.damage;
using Battle.network.actions.others;
using Battle.network.actions.user;
using Battle.network.packets;
using Core.models.enums;
using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace Battle.network
{
    public class BattleHandler
    {
        private readonly UdpClient _client;
        private readonly IPEndPoint remoteEP;
        private readonly int Players = 16;
        private string ipv4;
        public BattleHandler(UdpClient client, string ipv4, byte[] buff, IPEndPoint remote, DateTime date)
        {
            _client = client;
            remoteEP = remote;
            this.ipv4 = ipv4;
            BeginReceive(buff, date);
        }
        public void BeginReceive(byte[] buffer, DateTime date)
        {
            PacketModel packet = new PacketModel { _data = buffer, _receiveDate = date };
            ReceivePacket rec = new ReceivePacket(packet._data);
            packet._opcode = rec.readC();
            if (packet._opcode == 3 || packet._opcode == 4 || packet._opcode == 65 || packet._opcode == 67 || packet._opcode == 131 || packet._opcode == 132 || packet._opcode == 97)
            {
                packet._slot = rec.readC();
                packet._time = rec.readT();
                packet._round = rec.readC();
                packet._length = rec.readUH();
                packet._respawnNumber = rec.readC();
                packet._roundNumber = rec.readC();
                packet._accountId = rec.readC();
                packet._unkInfo2 = rec.readC();

                if (packet._length > packet._data.Length)
                    Console.WriteLine("Package Error!");
                else
                    ReadPacket(GetDecryptedData(packet));
            }
        }
        public PacketModel GetDecryptedData(PacketModel packet)
        {
            try
            {
                if (packet._data.Length >= packet._length)
                {
                    byte[] result = new byte[packet._length - 13];
                    Array.Copy(packet._data, 13, result, 0, result.Length);

                    byte[] withEnd = AllUtils.Decrypt(result, packet._length % 6 + 1);
                    byte[] noEnd = new byte[withEnd.Length - 9];
                    Array.Copy(withEnd, noEnd, noEnd.Length);
                    packet._withEndData = withEnd;
                    packet._noEndData = noEnd;
                }
            }
            catch
            {
                NextModel.AddOffet(ipv4);
                Battle_SyncNet.ExcptionPlayer(ipv4);
                GetPrestart.Bloqueando(ipv4);
            }
            return packet;
        }
        public void ReadPacket(PacketModel packet)
        {
            byte[] withEndData = packet._withEndData;
            byte[] noEndData = packet._noEndData;

            ReceivePacket rec = new ReceivePacket(withEndData);
            try
            {
                int BasicBufferLength = noEndData.Length, gen2 = 0, dedicationSlot = 0;
                uint UniqueRoomId = 0;
                Room room = null;
                switch (packet._opcode)
                {
                    case int _Opc when (_Opc == 131 || _Opc == 132):
                        rec.Advance(BasicBufferLength);
                        UniqueRoomId = rec.readUD();
                        dedicationSlot = rec.readC();
                        gen2 = rec.readD();
                        room = RoomsManager.GetRoom(UniqueRoomId);
                        if (room != null)
                        {
                            Player player = room.GetPlayer(packet._slot, remoteEP);
                            if (player != null && player.AccountIdIsValid(packet._accountId))
                            {
                                room._isBotMode = true;
                                Player p2 = dedicationSlot != byte.MaxValue ? room.GetPlayer(dedicationSlot, false) : null;
                                byte[] code;
                                if (p2 != null)
                                    code = Packet132Creator.GetCode132(noEndData, p2._date, packet._round, dedicationSlot);
                                else
                                    code = Packet132Creator.GetCode132(noEndData, player._date, packet._round, packet._slot);
                                int count = 0;
                                do
                                {
                                    Player playerR = room._players[count];
                                    if (playerR._client != null && playerR.AccountIdIsValid() && count != packet._slot)
                                        Send(code, playerR._client);
                                }
                                while (++count < Players);
                            }
                        }
                        break;
                    case 97:
                        UniqueRoomId = rec.readUD();
                        dedicationSlot = rec.readC();
                        gen2 = rec.readD();
                        room = RoomsManager.GetRoom(UniqueRoomId);
                        if (room != null)
                        {
                            Player player = room.GetPlayer(packet._slot, remoteEP);
                            if (player != null)
                            {
                                player.LastPing = packet._receiveDate;
                                Send(packet._data, remoteEP);
                            }
                        }
                        break;
                    case int _udp when (_udp == (int)SERVER_UDP_STATE.RELAY || _udp == (int)SERVER_UDP_STATE.RELAYCLIENT):
                        rec.Advance(BasicBufferLength);
                        UniqueRoomId = rec.readUD();
                        dedicationSlot = rec.readC();
                        gen2 = rec.readD();
                        room = RoomsManager.GetRoom(UniqueRoomId);
                        if (room != null)
                        {

                            Player player = room.GetPlayer(packet._slot, remoteEP);
                            if (player != null && player.AccountIdIsValid(packet._accountId))
                            {
                                player._respawnByUser = packet._respawnNumber;
                                if (packet._opcode == (int)SERVER_UDP_STATE.RELAYCLIENT)
                                    room._isBotMode = true;
                                if (room._startTime == new DateTime())
                                    return;
                                byte[] actions = WriteActionBytes(noEndData, room, AllUtils.GetDuration(player._date), packet);

                                bool useMyDate = packet._opcode == 4 && dedicationSlot == 255;

                                int valor = 0;
                                if (useMyDate)
                                    valor = packet._slot;
                                else if (packet._opcode == (int)SERVER_UDP_STATE.RELAY)
                                    valor = room._isBotMode ? packet._slot : byte.MaxValue;

                                byte[] code = Packet4Creator.GetCode4(actions, useMyDate ? player._date : room._startTime, packet._round, valor);
                                bool V3 = packet._opcode == 3 && !room._isBotMode && dedicationSlot != byte.MaxValue;
                                int playerslotcount = 0;
                                do
                                {
                                    bool V1 = playerslotcount != packet._slot;
                                    Player playerR = room._players[playerslotcount];
                                    if (playerR._client != null && playerR.AccountIdIsValid() && (dedicationSlot == 255 && V1 || packet._opcode == (int)SERVER_UDP_STATE.RELAY && room._isBotMode && V1 || V3))
                                        Send(code, playerR._client);
                                } while (++playerslotcount < Players);
                            }
                        }
                        break;
                    case 65:
                        string udpVersion = rec.readD() + "." + rec.readD();
                        UniqueRoomId = rec.readUD();
                        gen2 = rec.readD();
                        dedicationSlot = rec.readC();
                        room = RoomsManager.CreateOrGetRoom(UniqueRoomId, gen2);
                        if (room != null)
                        {
                            Player player = room.AddPlayer(remoteEP, packet, udpVersion, ipv4);
                            if (player != null)
                            {
                                if (!player.Integrity)
                                    player.ResetBattleInfos();
                                Send(Packet66Creator.GetCode66(), player._client);
                                    //Logger.Warning("Jogador se conectou ao Battle [" + player._client.Address + ":" + player._client.Port + "]");
                            }
                        }
                        else
                            Battle_SyncNet.ExcptionPlayer(ipv4);
                        break;

                    case 67:
                        byte[] unk = rec.readB(8);
                        UniqueRoomId = rec.readUD();
                        gen2 = rec.readD();
                        dedicationSlot = rec.readC();
                        room = RoomsManager.GetRoom(UniqueRoomId);
                        if (room != null)
                        {
                            room.RemovePlayer(packet._slot, remoteEP);
                            if (room.GetPlayersCount() == 0)
                                RoomsManager.RemoveRoom(room.UniqueRoomId);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //    Logger.Warning(ex.ToString());
                //   Logger.Warning("[Decrypted] Data: " + BitConverter.ToString(withEndData));
                //   Logger.Warning("[Encrypted] Data: " + BitConverter.ToString(packet._data));
                Battle_SyncNet.ExcptionPlayer(ipv4);
            }
        }
        private void RemoveHit(IList list, int idx)
        {
            list.RemoveAt(idx);
        }
        public List<ObjectHitInfo> TodasAcoesdaUDP(ActionModel ac, Room room, float time, out byte[] EventsData)
        {
            EventsData = new byte[0];
            if (room == null)
                return null;
            if (ac._data.Length == 0)
                return new List<ObjectHitInfo>();
            byte[] data = ac._data;
            List<ObjectHitInfo> objs = new List<ObjectHitInfo>();
            ReceivePacket rec = new ReceivePacket(data);
            using (SendPacket sp = new SendPacket())
            {
                int contador = 0;
                Player pl = room.GetPlayer(ac._slot, true);
                if (ac._flags.HasFlag(Events.ActionState))
                {
                    contador++;
                    PacoteDesconhecido_1.Struct info = PacoteDesconhecido_1.ReadInfo(rec, false);
                    PacoteDesconhecido_1.WriteInfo(sp, info);
                }
                if (ac._flags.HasFlag(Events.Animation))
                {
                    contador += 2;
                    a2_unk.writeInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.PosRotation))
                {
                    contador += 4;
                    a4_PositionSync.Struct info = a4_PositionSync.ReadInfo(rec, false);
                    a4_PositionSync.WriteInfo(sp, info);
                    if (pl != null)
                    {
                        pl.Position = new Half3(info._rotationX, info._rotationY, info._rotationZ);
                        // Battle_SyncNet.SendLocalSync(room, pl, info._cameraX, info._cameraY);
                    }
                }
                if (ac._flags.HasFlag(Events.OnLoadObject))
                {
                    contador += 8;
                    a8_MoveSync.Struct info = a8_MoveSync.ReadSyncInfo(ac, rec, false);
                    a8_MoveSync.WriteInfo(sp, info);
                    if (!room._isBotMode && info._objId != 65535 && (info._spaceFlags.HasFlag(CharaMoves.HELI_STOPPED) || info._spaceFlags.HasFlag(CharaMoves.HELI_IN_MOVE))) //16 entrando no heli em movimento | 96 saindo do heli | 128 entrando no heli parado
                    {
                        bool securityBlock = false;
                        ObjectInfo obj = room.GetObject(info._objId);
                        if (obj != null)
                        {
                            if (info._spaceFlags.HasFlag(CharaMoves.HELI_STOPPED))
                            {
                                AnimModel anim = obj._anim;
                                if (anim != null && anim._id == 0)
                                    obj._model.GetAnim(anim._nextAnim, 0, 0, obj);
                            }
                            else if (info._spaceFlags.HasFlag(CharaMoves.HELI_IN_MOVE))
                            {
                                DateTime date = obj._useDate;
                                if (date.ToString("yyMMddHHmm") == "0101010000")
                                    securityBlock = true;
                            }
                            if (!securityBlock)
                                objs.Add(new ObjectHitInfo(3)
                                {
                                    objSyncId = 1,
                                    objId = obj._id,
                                    objLife = obj._life,
                                    _animId1 = 255,
                                    _animId2 = obj._anim != null ? obj._anim._id : 255,
                                    _specialUse = AllUtils.GetDuration(obj._useDate)
                                });
                        }
                    }
                    info = null;
                }
                if (ac._flags.HasFlag(Events.Unk1))
                {
                    contador += 0x10;
                    PacoteDesconhecido_10.WriteInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.RadioChat))
                {
                    contador += 0x20;
                    a20_RadioSync.writeInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.WeaponSync))
                {
                    contador += 0x40;
                    a40_WeaponSync.Struct info = a40_WeaponSync.ReadInfo(ac, rec, false);
                    a40_WeaponSync.writeInfo(sp, info);
                    if (pl != null)
                    {
                        pl.WeaponClass = (ClassType)info.WeaponClass;
                        pl.WeaponSlot = info.WeaponSlot;
                        pl._character = (CHARACTER_RES_ID)info._charaModelId;
                        room.SyncInfo(objs, 2);
                    }
                }
                if (ac._flags.HasFlag(Events.WeaponRecoil))
                {
                    contador += 0x80;
                    a80_WeaponRecoil.writeInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.LifeSync))
                {
                    contador += 0x100;
                    a100_LifeSync.writeInfo(sp, rec, false);
                }
                if (ac._flags.HasFlag(Events.Suicide))
                {
                    contador += 0x200;
                    List<a200_SuicideDamage.HitData> hits = a200_SuicideDamage.ReadInfo(rec, false);
                    int basicInfo = 0;
                    if (pl != null)
                    {
                        List<DeathServerData> deaths = new List<DeathServerData>();
                        int ObjIdx = -1;
                        for (int i = 0; i < hits.Count; i++)
                        {
                            a200_SuicideDamage.HitData hit = hits[i];
                            if (pl != null && !pl.isDead && pl._life > 0)
                            {
                                int damage = (int)(hit._hitInfo >> 20);
                                CHARA_DEATH deathType = (CHARA_DEATH)(hit._hitInfo & 15);
                                int killerId = (int)((hit._hitInfo >> 11) & 511);
                                int hitPart = (int)((hit._hitInfo >> 4) & 63);
                                int unk = (int)((hit._hitInfo >> 10) & 1); //0 = User | 1 = Object
                                if (unk == 1)
                                    ObjIdx = killerId;

                                basicInfo = AllUtils.CreateItemId(AllUtils.ItemClass(hit.WeaponClass), hit._weaponSlot, (int)hit.WeaponClass, hit.WeaponId);

                                pl._life -= damage;
                                if (pl._life <= 0)
                                    DamageManager.SetDeath(deaths, pl, deathType);
                                else
                                    DamageManager.SetHitEffect(objs, pl, deathType, hitPart);


                                objs.Add(new ObjectHitInfo(2)
                                {
                                    objId = pl._slot,
                                    objLife = pl._life,
                                    deathType = deathType,
                                    hitPart = hitPart,
                                    weaponId = basicInfo,
                                    Position = hit.PlayerPos
                                });
                            }
                            else RemoveHit(hits, i--);
                        }
                        if (deaths.Count > 0)
                            Battle_SyncNet.SendDeathSync(room, pl, ObjIdx, basicInfo, deaths);
                        deaths = null;
                    }
                    else hits = new List<a200_SuicideDamage.HitData>();
                    a200_SuicideDamage.writeInfo(sp, hits);
                    hits = null;
                }
                if (ac._flags.HasFlag(Events.Mission))
                {
                    contador += 0x400;
                    a400_Mission.Struct info = a400_Mission.ReadInfo(ac, rec, false, time);
                    if (room.Map != null && pl != null && !pl.isDead && info._plantTime > 0 &&
                        !info.BombEnum.HasFlag(BombFlag.Stop))
                    {
                        BombPosition bomb = room.Map.GetBomb(info.BombId);
                        if (bomb != null)
                        {
                            bool isDefuse = info.BombEnum.HasFlag(BombFlag.Defuse);
                            Vector3 BombVec3d;
                            if (isDefuse)
                                BombVec3d = room.BombPosition;
                            else if (info.BombEnum.HasFlag(BombFlag.Start))
                                BombVec3d = bomb.Position;
                            else
                                BombVec3d = new Half3(0, 0, 0);
                            double PlayerDistance = Vector3.Distance(pl.Position, BombVec3d);
                            if ((bomb.Everywhere || PlayerDistance <= 2.0) &&
                                (pl._team == 1 && isDefuse || pl._team == 0 && !isDefuse))
                            {
                                if (pl._C4FTime != info._plantTime)
                                {
                                    pl._C4First = DateTime.Now;
                                    pl._C4FTime = info._plantTime;
                                }
                                double Seconds = (DateTime.Now - pl._C4First).TotalSeconds;

                                float objective = isDefuse ? pl._defuseDuration : pl._plantDuration;

                                if (((time >= info._plantTime + (objective)) || Seconds >= objective) &&
                                    (!room._hasC4 && info.BombEnum.HasFlag(BombFlag.Start) ||
                                        room._hasC4 && isDefuse))
                                {
                                    room._hasC4 = !room._hasC4;
                                    info._bombAll |= 2;
                                    a400_Mission.SendC4UseSync(room, pl, info);
                                }
                            }
                        }
                    }
                    a400_Mission.WriteInfo(sp, info);
                    info = null;
                }
                if (ac._flags.HasFlag(Events.TakeWeapon))
                {
                    contador += 0x800;
                    a800_WeaponAmmo.writeInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.DropWeapon))
                {
                    contador += 0x1000;
                    if (room != null && !room._isBotMode)
                    {
                        room._dropCounter++;
                        if (room._dropCounter > Config.maxDrop)
                            room._dropCounter = 0;
                    }
                    a1000_DropWeapon.writeInfo(sp, rec, false, room != null ? room._dropCounter : 0);
                }
                if (ac._flags.HasFlag(Events.FireSync))
                {
                    contador += 0x2000;
                    a2000_FireSync.writeInfo(sp, rec, false);
                }
                if (ac._flags.HasFlag(Events.AIDamage))
                {
                    contador += 0x4000;
                    a4000_BotHitData.WriteInfo(sp, rec, false);
                }
                if (ac._flags.HasFlag(Events.NormalDamage))
                {
                    contador += 0x8000;
                    List<a8000_NormalHitData.HitData> hits = a8000_NormalHitData.ReadInfo(rec, false);
                    List<DeathServerData> deaths = new List<DeathServerData>();
                    int basicInfo = 0;
                    if (pl != null)
                    {
                        for (int i = 0; i < hits.Count; i++)
                        {
                            a8000_NormalHitData.HitData hit = hits[i];
                            if (hit.HitEnum == HitType.HelmetProtection || hit.HitEnum == HitType.HeadshotProtection)
                                continue;
                            double HitDistance = Vector3.Distance(hit.StartBullet, hit.EndBullet);
                            if (hit._weaponSlot == 2 && (MeleeExceptionsJSON.Contains(hit.WeaponId) || HitDistance < 3) || hit._weaponSlot != 2)
                            {
                                int damage = AllUtils.GetHitDamageNORMAL(hit._hitInfo), objId = AllUtils.GetHitWho(hit._hitInfo), hitPart = AllUtils.GetHitPart(hit._hitInfo);
                                CHARA_DEATH deathType = CHARA_DEATH.DEFAULT;
                                basicInfo = AllUtils.CreateItemId(AllUtils.ItemClass(hit.WeaponClass), hit._weaponSlot, (int)hit.WeaponClass, hit.WeaponId);
                                ObjectType hitType = AllUtils.GetHitType(hit._hitInfo);
                                switch (hitType)
                                {
                                    case ObjectType.Object:
                                        {
                                            ObjectInfo obj = room.GetObject(objId);
                                            ObjModel objM = obj?._model;
                                            if (objM != null && objM.isDestroyable)
                                            {
                                                if (obj._life > 0)
                                                {
                                                    obj._life -= damage;
                                                    if (obj._life <= 0)
                                                    {
                                                        obj._life = 0;
                                                        DamageManager.BoomDeath(room, pl, basicInfo, deaths, objs, hit.BoomPlayers);
                                                    }
                                                    obj.DestroyState = objM.CheckDestroyState(obj._life);
                                                    DamageManager.SabotageDestroy(room, pl, objM, obj, damage);
                                                    objs.Add(new ObjectHitInfo(objM._updateId)
                                                    {
                                                        objId = obj._id,
                                                        objLife = obj._life,
                                                        killerId = ac._slot,
                                                        objSyncId = objM._needSync ? 1 : 0,
                                                        _specialUse = AllUtils.GetDuration(obj._useDate),
                                                        _animId1 = objM._anim1,
                                                        _animId2 = obj._anim != null ? obj._anim._id : 255,
                                                        _destroyState = obj.DestroyState
                                                    });
                                                }
                                            }
                                            break;
                                        }
                                    case ObjectType.User:
                                        {
                                            if (room.GetPlayer(objId, out Player player) && !pl.isDead && !player.isDead)
                                            {
                                                if (!player.Immortal)
                                                {
                                                    Battle_SyncNet.SendHitMarkerSync(room, pl, (int)deathType, (int)hit.HitEnum, damage, player._life, basicInfo);
                                                    if (hitPart == 29)
                                                        deathType = CHARA_DEATH.HEADSHOT;
                                                    if (room.stageType == 8 && deathType != CHARA_DEATH.HEADSHOT)
                                                        damage = 1;
                                                    else if (room.stageType == 7 && deathType == CHARA_DEATH.HEADSHOT)
                                                    {
                                                        if (room.LastRound == 1 && player._team == 0 || room.LastRound == 2 && player._team == 1)
                                                            damage = (damage / 10);
                                                    }
                                                    else if (room.stageType == 13)
                                                        damage = 200;
                                                    int valor = player._life -= damage;

                                                    DamageManager.SimpleDeath(deaths, objs, pl, player, valor, basicInfo, hitPart, deathType);

                                                    if (Config.useHitMarker)
                                                    {
                                                        if (damage > 20)
                                                            LoggerHacker.HackerCheck("Nick:" + pl.isNick + " ||" + damage + "Dano" + " ||" + room._roomId + "Sala'" + " ||" + hit.WeaponId + "Weapon");
                                                       
                                                    }

                                                }
                                                else
                                                {
                                                    Battle_SyncNet.SendHitMarkerSync(room, pl, (int)deathType, 4, damage, 999, basicInfo);
                                                    RemoveHit(hits, i--);
                                                }
                                            }

                                            else
                                            {
                                                RemoveHit(hits, i--);
                                            }
                                                break;
                                            }
                                    case ObjectType.UserObject:
                                        {
                                            int ownerSlot = (objId >> 4);
                                            int grenadeMapId = (objId & 15);
                                            break;
                                        }
                                    default:
                                        {
                                            Logger.Warning("[Warning] A new hit type: (" + hitType + "/" + (int)hitType + "); by slot: " + ac._slot);
                                            Logger.Warning("[Warning] BoomPlayers: " + hit._boomInfo + ";" + hit.BoomPlayers.Count);
                                            break;
                                        }
                                }
                            }
                            else
                                RemoveHit(hits, i--);
                        }
                        if (deaths.Count > 0)
                            Battle_SyncNet.SendDeathSync(room, pl, 255, basicInfo, deaths);
                    }
                    else hits = new List<a8000_NormalHitData.HitData>();
                    a8000_NormalHitData.writeInfo(sp, hits);
                    deaths = null;
                    hits = null;
                }
                if (ac._flags.HasFlag(Events.BoomDamage))//BARRIL EFEITO E DANO
                {
                    contador += 0x10000;
                    List<a10000_BoomHitData.HitData> hits = a10000_BoomHitData.ReadInfo(rec, false);
                    List<DeathServerData> deaths = new List<DeathServerData>();
                    int basicInfo = 0;
                    if (pl != null)
                    {
                        int idx = -1;
                        for (int i = 0; i < hits.Count; i++)
                        {
                            a10000_BoomHitData.HitData hit = hits[i];
                            int damage = AllUtils.GetHitDamageNORMAL(hit._hitInfo),
                                objId = AllUtils.GetHitWho(hit._hitInfo),
                                hitPart = AllUtils.GetHitPart(hit._hitInfo);
                            basicInfo = AllUtils.CreateItemId(AllUtils.ItemClass(hit.WeaponClass), hit._weaponSlot, (int)hit.WeaponClass, hit.WeaponId);
                            ObjectType hitType = AllUtils.GetHitType(hit._hitInfo);
                            switch (hitType)
                            {
                                case ObjectType.Object:
                                    ObjectInfo obj = room.GetObject(objId);
                                    ObjModel objM = obj?._model;
                                    if (objM != null && objM.isDestroyable && obj._life > 0)
                                    {
                                        obj._life -= damage;
                                        if (obj._life <= 0)
                                        {
                                            obj._life = 0;
                                            DamageManager.BoomDeath(room, pl, basicInfo, deaths, objs, hit.BoomPlayers);
                                        }
                                        obj.DestroyState = objM.CheckDestroyState(obj._life);
                                        DamageManager.SabotageDestroy(room, pl, objM, obj, damage);
                                        if (damage > 0)
                                            objs.Add(new ObjectHitInfo(objM._updateId)
                                            {
                                                objId = obj._id,
                                                objLife = obj._life,
                                                killerId = ac._slot,
                                                objSyncId = objM._needSync ? 1 : 0,
                                                _animId1 = objM._anim1,
                                                _animId2 = obj._anim != null ? obj._anim._id : 255,
                                                _destroyState = obj.DestroyState,
                                                _specialUse = AllUtils.GetDuration(obj._useDate),
                                            });
                                    }
                                    break;
                                case ObjectType.User:
                                    idx++;
                                    if (damage > 0 && room.GetPlayer(objId, out Player player) && !player.isDead)
                                    {
                                        if (hit._deathType == 10)
                                        {
                                            player._life += damage;
                                            player.CheckLifeValue();
                                        }
                                        else if (hit._deathType == 2 && ClassType.Dino != hit.WeaponClass && (idx % 2 == 0))
                                        {
                                            damage = (int)Math.Ceiling(damage / 2.7);// + 14;
                                                                                     // Console.WriteLine("The server is reduced from " + valor + " to " + damage + " damage by Explosion.");
                                            player._life -= damage;
                                            if (player._life <= 0)
                                                DamageManager.SetDeath(deaths, player, (CHARA_DEATH)hit._deathType);
                                            else
                                                DamageManager.SetHitEffect(objs, player, pl, (CHARA_DEATH)hit._deathType, hitPart);
                                        }
                                        else
                                        {
                                            player._life -= damage;
                                            if (player._life <= 0)
                                                DamageManager.SetDeath(deaths, player, (CHARA_DEATH)hit._deathType);
                                            else
                                                DamageManager.SetHitEffect(objs, player, pl, (CHARA_DEATH)hit._deathType, hitPart);
                                        }
                                        if (damage > 0)
                                        {
                                            Battle_SyncNet.SendHitMarkerSync(room, pl, hit._deathType, (int)hit.HitEnum, damage, player._life, basicInfo);
                                            objs.Add(new ObjectHitInfo(2)
                                            {
                                                objId = player._slot,
                                                objLife = player._life,
                                                deathType = (CHARA_DEATH)hit._deathType,
                                                weaponId = basicInfo,
                                                hitPart = hitPart
                                            });
                                        }
                                    }
                                    else
                                        RemoveHit(hits, i--);
                                    break;
                                case ObjectType.UserObject:
                                    int ownerSlot = (objId >> 4);
                                    int grenadeMapId = (objId & 15);
                                    break;
                                default:
                                    {
                                        Logger.Warning("Grenade BOOM, new hit type: (" + hitType + "/" + (int)hitType + ")");
                                        break;
                                    }
                            }
                        }
                        if (deaths.Count > 0)
                            Battle_SyncNet.SendDeathSync(room, pl, 255, basicInfo, deaths);
                    }
                    else hits = new List<a10000_BoomHitData.HitData>();
                    a10000_BoomHitData.writeInfo(sp, hits);
                    deaths = null;
                    hits = null;
                }
                if (ac._flags.HasFlag(Events.Death))
                {
                    contador += 0x40000;
                    a40000_DeathData.writeInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.SufferingDamage))
                {
                    contador += 0x80000;
                    a80000_SufferingDamage.WriteInfo(sp, ac, rec, false);
                }
                if (ac._flags.HasFlag(Events.PassPortal)) //DINO
                {
                    contador += 0x100000;
                    a100000_PassPortal.Struct info = a100000_PassPortal.ReadInfo(ac, rec, false);
                    a100000_PassPortal.writeInfo(sp, info);
                    if (pl != null && !pl.isDead)
                        a100000_PassPortal.SendPassSync(room, pl, info);
                    info = null;
                }
                EventsData = sp.mstream.ToArray();
                if (contador != (uint)ac._flags)
                    Logger.Warning("[" + (uint)ac._flags + "|" + ((uint)ac._flags - contador) + ", BUF ANORMAL3: " + BitConverter.ToString(data));
                return objs;
            }
        }
        public void CheckDataFlags(ActionModel ac, PacketModel packet)
        {
            Events flags = ac._flags;
            if (!flags.HasFlag(Events.WeaponSync) || packet._opcode == 4)
                return;
            if ((flags & (Events.TakeWeapon | Events.DropWeapon)) > 0)
                ac._flags -= Events.WeaponSync;
        }
        public byte[] WriteActionBytes(byte[] data, Room room, float time, PacketModel packet)
        {
            ReceivePacket p = new ReceivePacket(data);
            List<ObjectHitInfo> objs = new List<ObjectHitInfo>();
            using (SendPacket s = new SendPacket())
            {
                for (int i = 0; i < Players; i++)
                {
                    ActionModel ac = new ActionModel();
                    try
                    {
                        ac._type = (P2P_SUB_HEAD)p.readC(out bool exception);
                        if (exception) break;
                        ac._slot = p.readUH();
                        ac._lengthData = p.readUH();
                        if (ac._lengthData == ushort.MaxValue)
                            break;
                        s.WriteC((byte)ac._type);
                        s.WriteH(ac._slot);
                        s.WriteH(ac._lengthData);
                        switch (ac._type)
                        {
                            case P2P_SUB_HEAD.GRENADE:
                                Code1_GrenadeSync.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.DROPEDWEAPON:
                                Code2_WeaponSync.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.OBJECT_STATIC:
                                code3_ObjectStatic.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.OBJECT_ANIM:
                                Code6_ObjectAnim.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.STAGEINFO_OBJ_STATIC:
                                Code9_StageInfoObjStatic.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.STAGEINFO_OBJ_ANIM:
                                Code12_StageObjAnim.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD.CONTROLED_OBJECT:
                                code13_ControledObj.WriteInfo(s, p, false);
                                break;
                            case P2P_SUB_HEAD p2P_SUB_HEAD when (p2P_SUB_HEAD == P2P_SUB_HEAD.USER || p2P_SUB_HEAD == P2P_SUB_HEAD.STAGEINFO_CHARA):
                                {
                                    ac._flags = (Events)p.readUD();
                                    ac._data = p.readB(ac._lengthData - 9);
                                    CheckDataFlags(ac, packet);
                                    objs.AddRange(TodasAcoesdaUDP(ac, room, time, out byte[] result));
                                    s.GoBack(2);
                                    s.WriteH((ushort)(result.Length + 9));
                                    s.WriteD((uint)ac._flags);
                                    s.WriteB(result);
                                    if (ac._data.Length == 0 && (ac._lengthData - 9 != 0))
                                        break;
                                    break;
                                }
                            default:
                                {
                                    Logger.Warning("[New user packet type '" + ac._type + "' or '" + (int)ac._type + "']: " + BitConverter.ToString(data));
                                    throw new Exception("Unknown action type");
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning("[WriteActionBytes]\r\n" + ex.ToString());
                        Logger.Warning("[WAB] Data: " + BitConverter.ToString(data));
                        objs = new List<ObjectHitInfo>();
                        break;
                    }
                }
                if (objs.Count > 0)
                    s.WriteB(Packet4Creator.GetCode4SyncData(objs));
                return s.mstream.ToArray();
            }
        }
        private void Send(byte[] data, IPEndPoint ip)
        {
            _client.Send(data, data.Length, ip);
        }
    }
}