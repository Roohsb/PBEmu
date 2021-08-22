using Battle.config;
using Battle.data.enums;
using Battle.data.enums.weapon;
using SharpDX;
using System;
using System.Net;

namespace Battle.data.models
{
    public class Player
    {
        public int _slot = -1, _team, _life = 100, _maxLife = 100,
            _playerIdByUser = -2, _playerIdByServer = -1, WeaponSlot,
            _respawnByUser = -2, _respawnByLogic, _respawnByServer = -1;
        public float _plantDuration, _defuseDuration, _C4FTime;
        public Half3 Position;
        public IPEndPoint _client;
        public DateTime _date, LastPing, LastDie, _C4First;
        public ClassType WeaponClass;
        public CHARACTER_RES_ID _character;
        public string isNick = "";
        public bool isDead = false, _neverRespawn = true, Integrity = true, Immortal = false, _LifeAdut = false;
        public Player(int slot)
        {
            _slot = slot;
            _team = (slot % 2);
        }
        public bool CompareIP(IPEndPoint ip) =>
            _client != null && ip != null && _client.Address.Equals(ip.Address) && _client.Port == ip.Port;
        public bool RespawnIsValid() =>  _respawnByServer == _respawnByUser;
        public bool RespawnLogicIsValid() => _respawnByServer == _respawnByLogic;
        public bool AccountIdIsValid() => _playerIdByServer == _playerIdByUser;
        public bool AccountIdIsValid(int number) => (_playerIdByServer == number);
        public void CheckLifeValue()
        {
            if (_life > _maxLife)
                _life = _maxLife;
        }
        public void ResetAllInfos()
        {
            _client = null;
            _date = new DateTime();
            _playerIdByUser = -2;
            _playerIdByServer = -1;
            Integrity = true;
            ResetBattleInfos();
        }
        public void ResetBattleInfos()
        {
            _respawnByServer = -1;
            _respawnByUser = -2;
            _respawnByLogic = 0;
            Immortal = false;
            isDead = true;
            _neverRespawn = true;
            WeaponClass = ClassType.Unknown;
            WeaponSlot = 0;
            LastPing = new DateTime();
            LastDie = new DateTime();
            _C4First = new DateTime();
            _C4FTime = 0;
            Position = new Half3();
            _life = 100;
            _maxLife = 100;
            _LifeAdut = false;
            _plantDuration = Config.plantDuration;
            _defuseDuration = Config.defuseDuration;
        }
        public void ResetLife()
        {
            _life = _maxLife;
        }
    }
}