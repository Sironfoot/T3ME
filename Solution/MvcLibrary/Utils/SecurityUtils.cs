using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MvcLibrary.Utils
{
    public enum KeyStrength
    {
        _128bit = 128,
        _160bit = 160,
        _192bit = 192,
        _256bit = 256,
        _512bit = 512,
        /// <summary>
        ///     Generates a 1024-bit key. Key sizes beyond this don't
        ///     make sense (unless you invent a quantum computer)
        /// </summary>
        _1024bit = 1024
    }

    public static class SecurityUtils
    {
        public static string GenerateSecureKey(KeyStrength keyStrength)
        {
            int numBytes = (int)Math.Ceiling((double)keyStrength / 8.0);

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[numBytes];

            crypto.GetBytes(bytes);

            return BytesToHexString(bytes);
        }

        private static string BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length * 2);

            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }

            return hexString.ToString();
        }
    }
}