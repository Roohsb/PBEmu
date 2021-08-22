using Microsoft.Win32.SafeHandles;
using SharpDX;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Battle.network
{
    public class SendPacket : IDisposable
    {
        public MemoryStream mstream = new MemoryStream();
        bool disposed = false;
        public readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            mstream.Dispose();
            if (disposing)
                handle.Dispose();
            disposed = true;
        }
        protected internal void WriteB(byte[] value)
        {
            mstream.Write(value, 0, value.Length);
        }
        protected internal void WriteD(int value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteD(uint value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteH(short val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteH(ushort val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteC(bool value)
        {
            mstream.WriteByte(Convert.ToByte(value));
        }
        protected internal void WriteC(byte value)
        {
            mstream.WriteByte(value);
        }
        protected internal void WriteF(double value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteT(float value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteQ(long value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteHVector(Half3 half)
        {
            WriteH(half.X.RawValue);
            WriteH(half.Y.RawValue);
            WriteH(half.Z.RawValue);
        }
        protected internal void WriteTVector(Half3 half)
        {
            WriteT(half.X);
            WriteT(half.Y);
            WriteT(half.Z);
        }
        protected internal void WriteS(string value)
        {
            if (value != null)
                WriteB(Encoding.Unicode.GetBytes(value));
            WriteH(0);
        }
        protected internal void WriteS(string name, int count)
        {
            if (name == null)
                return;
            WriteB(Encoding.GetEncoding(1251).GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }
        /// <summary>
        /// Volta uma determinada quantia de bytes do MemoryStream.
        /// </summary>
        /// <param name="value"></param>
        protected internal void GoBack(int value)
        {
            mstream.Position -= value;
        }
    }
}