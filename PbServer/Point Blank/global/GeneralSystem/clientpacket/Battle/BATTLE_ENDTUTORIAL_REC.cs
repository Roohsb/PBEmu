using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_ENDTUTORIAL_REC : ReceiveGamePacket
    {
        public BATTLE_ENDTUTORIAL_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            _client.SendPacket(new BATTLE_TUTORIAL_ROUND_END_PAK());
            _client.SendPacket(new BATTLE_ENDBATTLE_PAK(_client._player));
        }
    }
}