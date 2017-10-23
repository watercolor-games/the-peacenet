using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Plex.Objects
{
    public sealed class DebugBinaryWriter : BinaryWriter
    {
        public DebugBinaryWriter(Stream str) : base(str)
        {

        }

        public override void Write(bool value)
        {
            Debug.Print("Wrote bool: " + value);
            base.Write(value);
        }

        public override void Write(byte value)
        {
            Debug.Print("Wrote byte: " + value);
            base.Write(value);
        }

        public override void Write(byte[] value)
        {
            Debug.Print("Wrote bytes: " + value.Length);
            base.Write(value);
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            Debug.Print("Wrote bytes: " + buffer.Length + ", " + index + ", " + count);

            base.Write(buffer, index, count);
        }

        public override void Write(char ch)
        {
            Debug.Print("Wrote char: " + ch);
            base.Write(ch);
        }

        public override void Write(char[] value)
        {
            Debug.Print("Wrote chars: " + value.Length);
            base.Write(value);
        }

        public override void Write(char[] chars, int index, int count)
        {
            Debug.Print("Wrote chars: " + chars.Length + ", " + index + ", " + count);
            base.Write(chars, index, count);
        }

        public override void Write(decimal value)
        {
            Debug.Print("Wrote decimal: " + value);
            base.Write(value);
        }

        public override void Write(double value)
        {
            Debug.Print("Wrote double-precision float: " + value);
            base.Write(value);
        }

        public override void Write(float value)
        {
            Debug.Print("Wrote float: " + value);
            base.Write(value);
        }

        public override void Write(int value)
        {
            Debug.Print("Wrote 32bit int: " + value);
            base.Write(value);
        }

        public override void Write(long value)
        {
            Debug.Print("Wrote 64bit int: " + value);
            base.Write(value);
        }

        public override void Write(sbyte value)
        {
            Debug.Print("Wrote sbyte: " + value);
            base.Write(value);
        }

        public override void Write(short value)
        {
            Debug.Print("Wrote short: " + value);
            base.Write(value);
        }

        public override void Write(string value)
        {
            Debug.Print("Wrote string: " + value);
            base.Write(value);
        }

        public override void Write(uint value)
        {
            Debug.Print("Wrote unsigned 32bit int: " + value);
            base.Write(value);
        }

        public override void Write(ulong value)
        {
            Debug.Print("Wrote unsigned 64bit int: " + value);
            base.Write(value);
        }

        public override void Write(ushort value)
        {
            Debug.Print("Wrote unsigned short: " + value);
            base.Write(value);
        }
    }
}
