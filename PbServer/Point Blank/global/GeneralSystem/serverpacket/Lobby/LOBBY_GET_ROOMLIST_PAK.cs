using Core.server;

namespace Game.global.serverpacket
{
    public class LOBBY_GET_ROOMLIST_PAK : SendPacket
    {
        private int _roomPage, _playerPage, _allPlayers, _allRooms, _count1, _count2;
        private byte[] _salas, _waiting;
        public LOBBY_GET_ROOMLIST_PAK(int allRooms, int allPlayers, int roomPage, int playerPage, int count1, int count2, byte[] rooms, byte[] players)
        {
            _allRooms = allRooms;
            _allPlayers = allPlayers;
            _roomPage = roomPage;
            _playerPage = playerPage;
            _salas = rooms;
            _waiting = players;
            _count1 = count1;
            _count2 = count2;
        }

        public override void Write()
        {
            WriteH(3074);
            WriteD(_allRooms); //total
            WriteD(_roomPage); //página atual(roomPages - 1)
            WriteD(_count1); //15 salas por página_salas.Count - carregando atualmente
            WriteB(_salas);

            WriteD(_allPlayers); //total
            WriteD(_playerPage); //página atual
            WriteD(_count2); //10 por página
            WriteB(_waiting);
        }
    }
}