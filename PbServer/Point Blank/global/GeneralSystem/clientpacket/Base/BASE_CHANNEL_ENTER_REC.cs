using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_CHANNEL_ENTER_REC : ReceiveGamePacket
    {
        private int channelId;
        public BASE_CHANNEL_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            channelId = ReadD();
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p == null || p.channelId >= 0)
                return;
            Channel ch = ChannelsXML.getChannel(channelId);
            if (ch != null)
            {
                if (EnterServer(ch, ChannelRequirementCheck(p, ch)))
                {
                    p.channelId = channelId;
                    _client.SendPacket(new BASE_CHANNEL_ENTER_PAK(p.channelId, ch._announce));
                    p._status.updateChannel((byte)p.channelId);
                    p.UpdateCacheInfo();
                }
            }
            else
                _client.SendPacket(new BASE_CHANNEL_ENTER_PAK(0x80000000));
        }
        public bool EnterServer(Channel ch, bool verificado)
        {
            try
            {
                if (verificado)
                {
                    _client.SendPacket(new BASE_CHANNEL_ENTER_PAK(0x80000202));
                    return false;
                }
                else if (ch._players.Count >= Settings.maxChannelPlayers)
                {
                    _client.SendPacket(new BASE_CHANNEL_ENTER_PAK(0x80000201));
                    return false;
                }
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        private bool ChannelRequirementCheck(Account p, Channel ch)
        {
            if (p.IsGM())
                return false;
            else
            {
                switch (ch._type)
                {
                    case 5: //Canal avançado | Entre capitão 1-hero
                        {
                            if (p._rank <= 25)
                                return true;
                            break;
                        }
                    case 4://Canal de clã | Precisa de clã (Menos GM)
                        {
                            if (p.clanId == 0)
                                return true;
                            break;
                        }
                    case 3://Canal iniciante1 | KD abaixo de 40%
                        {
                            if (p._statistic.GetKDRatio() > 40)
                                return true;
                            break;
                        }
                    case 2://Canal iniciante2 | Entre Novato-Cabo
                        {
                            if (p._rank >= 4)
                                return true;
                            break;
                        }
                    case -1://Canal Bloqueiado
                        {
                            return true;
                        }
                }
                return false;
            }
        }
    }
}