using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cizeta.TraMaAuth
{
    class PasswordManager
    {
        public string ExceptionMessage;

        private string CryptoKey;

        public PasswordManager(string cryptoKey)
        {
            this.CryptoKey = cryptoKey;
            this.ExceptionMessage = string.Empty;
        }

        #region Public methods

        public string EncodePassword(string password)
        {
            string ret = null;
            Cryptography.EncryptionAlgorithm = Cryptography.Algorithm.Rijndael;
            Cryptography.Key = CryptoKey;
            Cryptography.Encoding = Cryptography.EncodingType.HEX;
            try
            {
                Cryptography.EncryptString(password);
                ret = Cryptography.Content;
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                ret = string.Empty;
            }
            return ret;
        }

        public string DecodePassword(string password)
        {
            string ret = null;
            if (password == string.Empty)
            {
                ret = string.Empty;
            }
            else
            {
                Cryptography.EncryptionAlgorithm = Cryptography.Algorithm.Rijndael;
                Cryptography.Key = CryptoKey;
                Cryptography.Encoding = Cryptography.EncodingType.HEX;
                Cryptography.Content = password;
                try
                {
                    Cryptography.DecryptString();
                    ret = Cryptography.Content;
                }
                catch (Exception ex)
                {
                    ExceptionMessage = ex.Message;
                    ret = string.Empty;
                }
            }
            return ret;
        }

        public bool CheckPassword(string storedPassword, string inputPassword)
        {
            if ((EncodePassword(inputPassword) == storedPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
