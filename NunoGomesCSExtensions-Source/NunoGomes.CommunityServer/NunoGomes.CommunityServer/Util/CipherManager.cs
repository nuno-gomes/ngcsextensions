using System;
using System.Collections.Generic;
using System.Text;

namespace NunoGomes.CommunityServer.Util
{
    public static partial class CipherManager
    {
        private const string m_passPhrase = ".Net@Pt&Com2Us";        // can be any string
        private const string m_saltValue = "s@1tValue";        // can be any string
        private const string m_hashAlgorithm = "SHA1";             // can be "MD5"
        private const int m_passwordIterations = 3;                  // can be any number
        private const string m_initVector = "@PT9Net5Pt0ComH8"; // must be 16 bytes
        private const int m_keySize = 256;                // can be 192 or 128

        private static RijndaelSymmetricAlgorithm _manager = null;

        static CipherManager()
        {
            _manager = new RijndaelSymmetricAlgorithm(
                m_passPhrase,
                m_saltValue,
                m_hashAlgorithm,
                m_passwordIterations,
                m_initVector,
                m_keySize);
        }

        public static string Encrypt(string plainText)
        {
            return _manager.Encrypt(plainText);
        }

        public static string Decrypt(string cipherText)
        {
            return _manager.Decrypt(cipherText);
        }
    }

}
