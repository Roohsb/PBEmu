using Core;
using Game.data.managers;
using Game.data.model;

namespace Game.global.GeneralSystem.clientpacket
{
    public class A_3094_REC : ReceiveGamePacket
    {
        private uint sessionId;
        private string name;
        public A_3094_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            sessionId = ReadUD();
            name = ReadS(ReadC());
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            Account p = _client._player;
            Channel ch = p.GetChannel();
            if (ch == null || p._room != null || sessionId == uint.MaxValue)
                return;
            try
            {
                PlayerSession pS = ch.GetPlayer(sessionId);
                if (pS == null)
                    return;
                Account pC = AccountManager.GetAccount(pS._playerId, true);
                if (pC == null)
                    return;
                //Ativa quando usa "/EXIT (APELIDO)"
                SendDebug.SendInfo("[3094] SessionId: " + sessionId + "; Name: " + name);
                //_client.SendPacket(new A_3094_PAK());
            }
            catch
            {
            }
        }
    }
}