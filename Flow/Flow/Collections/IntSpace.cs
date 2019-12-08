using System;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public class IntSpace
    {
        private ulong[][][][] Array;
        public ulong Length { get; private set; }
        public bool this[int Index]
        {
            get
            {
                return Contains(Index);
            }
            set
            {
                if (value)
                    Add(Index);
                else
                    Remove(Index);
            }
        }
        public bool this[uint Index]
        {
            get
            {
                return Contains(Index);
            }
            set
            {
                if (value)
                    Add(Index);
                else
                    Remove(Index);
            }
        }
        private bool Add(byte[] Number)
        {
            if (Array == null)
                Array = new ulong[byte.MaxValue + 1][][][];
            if (Array[Number[3]] == null)
                Array[Number[3]] = new ulong[byte.MaxValue + 1][][];
            if (Array[Number[3]][Number[2]] == null)
                Array[Number[3]][Number[2]] = new ulong[byte.MaxValue + 1][];
            if (Array[Number[3]][Number[2]][Number[1]] == null)
                Array[Number[3]][Number[2]][Number[1]] = new ulong[4];
            int Remainder;
            var Quotient = Math.DivRem(Number[0], 64, out Remainder);
            ulong Value = (Array[Number[3]][Number[2]][Number[1]][Quotient] >> Remainder) & 1UL;
            if (Value == 0)
            {
                Array[Number[3]][Number[2]][Number[1]][Quotient] |= (1UL << Remainder);
                return true;
            }
            return false;
        }
        private bool Remove(byte[] Number)
        {
            if (Array == null)
                return false;
            if (Array[Number[3]] == null)
                return false;
            if (Array[Number[3]][Number[2]] == null)
                return false;
            if (Array[Number[3]][Number[2]][Number[1]] == null)
                return false;
            int Remainder;
            var Quotient = Math.DivRem(Number[0], 64, out Remainder);
            ulong Value = (Array[Number[3]][Number[2]][Number[1]][Quotient] >> Remainder) & 1UL;
            if (Value != 0)
            {
                Array[Number[3]][Number[2]][Number[1]][Quotient] &= ~(1UL << Remainder);
                return true;
            }
            return false;
        }
        private bool Contains(byte[] Number)
        {
            if (Array == null)
                return false;
            if (Array[Number[3]] == null)
                return false;
            if (Array[Number[3]][Number[2]] == null)
                return false;
            if (Array[Number[3]][Number[2]][Number[1]] == null)
                return false;
            int Remainder;
            var Quotient = Math.DivRem(Number[0], 64, out Remainder);
            return ((Array[Number[3]][Number[2]][Number[1]][Quotient] >> Remainder) & 1UL) != 0;
        }
        public bool Add(int Number)
        {
            if (Add(GetBytes(Number)))
            {
                Length += 1;
                return true;
            }
            else
                return false;
        }
        public bool Add(uint Number)
        {
            if (Add(GetBytes(Number)))
            {
                Length += 1;
                return true;
            }
            else
                return false;
        }
        public bool Remove(int Number)
        {
            if (Remove(GetBytes(Number)))
            {
                Length -= 1;
                return true;
            }
            else
                return false;
        }
        public bool Remove(uint Number)
        {
            if (Remove(GetBytes(Number)))
            {
                Length -= 1;
                return true;
            }
            else
                return false;
        }
        public bool Contains(int Number)
        {
            return Contains(GetBytes(Number));
        }
        public bool Contains(uint Number)
        {
            return Contains(GetBytes(Number));
        }
        public void Clear()
        {
            Length = 0;
            Array = null;
        }
        private static byte[] GetBytes(int Number)
        {
            var Bytes = BitConverter.GetBytes(Number);

            if (!BitConverter.IsLittleEndian)
                System.Array.Reverse(Bytes);
            return Bytes;
        }
        private static byte[] GetBytes(uint Number)
        {
            var Bytes = BitConverter.GetBytes(Number);
            if (!BitConverter.IsLittleEndian)
                System.Array.Reverse(Bytes);
            return Bytes;
        }
    }
}
