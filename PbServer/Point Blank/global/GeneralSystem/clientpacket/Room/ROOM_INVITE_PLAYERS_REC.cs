using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_INVITE_PLAYERS_REC : ReceiveGamePacket
    {
        private int PlayerCount;
        private uint erro;
        public ROOM_INVITE_PLAYERS_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            PlayerCount = ReadD();
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p != null && p._room != null)
            {
                Channel ch = p.GetChannel();
                if (ch != null)
                {
                    using ROOM_INVITE_SHOW_PAK packet = new ROOM_INVITE_SHOW_PAK(p, p._room);
                    byte[] data = packet.GetCompleteBytes("ROOM_INVITE_PLAYERS_REC");
                    for (int i = 0; i < PlayerCount; i++)
                    {
                        try
                        {
                            Account ps = AccountManager.GetAccount(ch.GetPlayer(ReadUD())._playerId, true);
                            if (ps != null)
                                ps.SendCompletePacket(data);
                        }
                        catch { }
                    }
                }
            }
            else erro = 0x80000000;
            _client.SendPacket(new ROOM_INVITE_RETURN_PAK(erro));
        }
    }
}