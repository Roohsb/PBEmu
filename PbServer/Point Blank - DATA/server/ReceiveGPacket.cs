using System;
using System.Text;

namespace Core.server
{
    public class ReceiveGPacket
    {
        private byte[] _buffer;
        private int _offset;
        public ReceiveGPacket(byte[] buff)
        {
            _buffer = buff;
        }
        public byte[] GetBuffer() => _buffer;
        public int ReadD()
        {
            int num = BitConverter.ToInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public uint ReadUD()
        {
            uint num = BitConverter.ToUInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public byte ReadC()
        {
            try
            {
                return _buffer[_offset++];
            }
            catch { return 0; }
        }

        public byte[] ReadB(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(_buffer, _offset, result, 0, Length);
            _offset += Length;
            return result;
        }

        public short ReadH()
        {
            short num = BitConverter.ToInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public ushort ReadUH()
        {
            ushort num = BitConverter.ToUInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public double ReadF()
        {
            double num = BitConverter.ToDouble(_buffer, _offset);
            _offset += 8;
            return num;
        }
        public float ReadT()
        {
            float num = BitConverter.ToSingle(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public long ReadQ()
        {
            long num = BitConverter.ToInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }
        public string ReadS(int Length)
        {
            string str = "";
            try
            {
                str = Encoding.GetEncoding(1251).GetString(_buffer, _offset, Length);
                int length = str.IndexOf((char)0);
                if (length != -1)
                    str = str.Substring(0, length);
                _offset += Length;
            }
            catch
            {
            }
            return str;
        }
        public string ReadS()
        {
            string result = "";
            try
            {
                int count = (_buffer.Length - _offset);
                result = Encoding.Unicode.GetString(_buffer, _offset, count);
                int idx = result.IndexOf(char.MinValue);
                if (idx != -1)
                    result = result.Substring(0, idx);
                _offset += result.Length + 1;
            }
            catch
            {
            }
            return result;
        }
    }
}