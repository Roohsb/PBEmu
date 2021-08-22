using Core.models.enums.errors;
using Core.server;

namespace Game.global.Authentication
{
    public class BASE_LOGIN_PAK : SendPacket
    {
        private uint _result;
        private string _login;
        private long _pId;
        public BASE_LOGIN_PAK(EventErrorEnum result, string login, long pId)
        {
            _result = (uint)result;
            _login = login;
            _pId = pId;
        }
        public BASE_LOGIN_PAK(int result, string login, long pId)
        {
            _result = (uint)result;
            _login = login;
            _pId = pId;
        }
        public override void Write()
        {
            WriteH(2564);
            WriteD(_result);
            WriteC(0);
            WriteQ(_pId);
            WriteC((byte)_login.Length);
            WriteS(_login, _login.Length);
            WriteC(0); //(Max = 127/128)
            WriteC(0); //(Max = 49/50)
        }
    }
}