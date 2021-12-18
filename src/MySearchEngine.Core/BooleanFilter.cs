using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MySearchEngine.Core
{
    public class BooleanFilter
    {
        private bool[] _bArray;
        public BooleanFilter(int capacity)
        {
            _bArray = new bool[capacity];
        }

        public BooleanFilter(bool[] init)
        {
            _bArray = init;
        }

        public bool TryAdd(string str)
        {
            var bytes = ASCIIEncoding.ASCII.GetBytes(str);
            var hash1 = Math.Abs(str.GetHashCode() & _bArray.Length);
            var hash2 = ToSHA256Hash(bytes);
            var hash3 = ToMD5Hash(bytes);

            if (_bArray[hash1] && _bArray[hash2] && _bArray[hash3])
                return false; // Already added

            _bArray[hash1] = true;
            _bArray[hash2] = true;
            _bArray[hash3] = true;
            return true;
        }

        private int ToSHA256Hash(byte[] content)
        {
            var tmpNewHash = new SHA256CryptoServiceProvider().ComputeHash(content);
            return Math.Abs(BitConverter.ToInt32(tmpNewHash, 0) % _bArray.Length);
        }

        private int ToMD5Hash(byte[] content)
        {
            var tmpNewHash = new MD5CryptoServiceProvider().ComputeHash(content);
            return Math.Abs(BitConverter.ToInt32(tmpNewHash, 0) % _bArray.Length);
        }
    }
}
